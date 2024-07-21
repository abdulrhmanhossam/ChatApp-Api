using AutoMapper;
using ChatApp.API.DTOs;
using ChatApp.API.Entities;
using ChatApp.API.Extensions;
using ChatApp.API.Helpers;
using ChatApp.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers;

[Authorize]
public class UsersController(IUnitOfWork unitOfWork, IMapper mapper,
    IPhotoService photoService) : BaseApiController
{

    [HttpGet]
    public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
    {
        userParams.CurrentUserName = User.GetUserName();
        
        var users = await unitOfWork.UserRepository.GetMembersAsync(userParams);

        Response.AddPaginationHeader(
            new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));

        return Ok(users);
    }

    [HttpGet("{username:alpha}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var user = await unitOfWork.UserRepository.GetMemberByNameAsync(username);

        if (user == null) 
            return NotFound();

        return user;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MemberDto>> GetUser(int id)
    {
        return await unitOfWork.UserRepository.GetMemberByIdAsync(id);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        var user = await unitOfWork.UserRepository
            .GetUserByUsernameAsync(User.GetUserName());

        if (user == null)
            return NotFound();

        
        mapper.Map(memberUpdateDto, user);

        if(await unitOfWork.Complete())
            return NoContent();

        return BadRequest("Faild to update user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await unitOfWork.UserRepository
            .GetUserByUsernameAsync(User.GetUserName());

        if (user == null)
            return NotFound();

        var result = await photoService.AddPhotoAsync(file);

        if (result.Error != null)
            return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (user.Photos.Count == 0)
            photo.IsMain = true;

        user.Photos.Add(photo);

        if (await unitOfWork.Complete())
        {
            return CreatedAtAction(nameof(GetUser),
                new { user.UserName }, mapper.Map<PhotoDto>(photo));
        }

        return BadRequest("Problem adding Photo");
    }

    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await unitOfWork.UserRepository
            .GetUserByUsernameAsync(User.GetUserName());

        if (user == null)
            return NotFound();

        var photo = user.Photos
            .FirstOrDefault(p => p.Id == photoId);

        if (photo == null) 
            return NotFound();

        if (photo.IsMain)
            return BadRequest("this is already your main photo");

        var currentMain = user.Photos
            .FirstOrDefault(p => p.IsMain);

        if (currentMain != null)
            currentMain.IsMain = false;

        photo.IsMain = true;

        if (await unitOfWork.Complete())
            return NoContent();

        return BadRequest("problem setting the main photo");
    }

    [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await unitOfWork.UserRepository
            .GetUserByUsernameAsync(User.GetUserName());

        var photo = user.Photos
            .FirstOrDefault(p => p.Id == photoId);

        if (photo == null) 
            return NotFound();

        if (photo.IsMain)
            return BadRequest("You cannot delete your main photo");

        if (photo.PublicId != null)
        {
            var result = await photoService
                .DeletePhotoAsync(photo.PublicId);

            if (result.Error != null)
                return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if (await unitOfWork.Complete())
            return Ok();

        return BadRequest("problem deleting photo");
    }
    
}
