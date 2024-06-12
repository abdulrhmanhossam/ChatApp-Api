using AutoMapper;
using ChatApp.API.DTOs;
using ChatApp.API.Entities;
using ChatApp.API.Extensions;
using ChatApp.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _photoService = photoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await _userRepository.GetMembersAsync();
        return Ok(users);
    }

    [HttpGet("{username:alpha}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        return await _userRepository.GetMemberByNameAsync(username);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MemberDto>> GetUser(int id)
    {
        return await _userRepository.GetMemberByIdAsync(id);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
        var user = await _userRepository
            .GetUserByUsernameAsync(User.GetUserName());

        if (user == null)
            return NotFound();

        
        _mapper.Map(memberUpdateDto, user);

        if(await _userRepository.SaveAllAsync())
            return NoContent();

        return BadRequest("Faild to update user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        var user = await _userRepository
            .GetUserByUsernameAsync(User.GetUserName());

        if (user == null)
            return NotFound();

        var result = await _photoService.AddPhotoAsync(file);

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

        if (await _userRepository.SaveAllAsync())
        {
            return CreatedAtAction(nameof(GetUser),
                new { user.UserName }, _mapper.Map<PhotoDto>(photo));
        }

        return BadRequest("Problem adding Photo");
    }

    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await _userRepository
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

        if (await _userRepository.SaveAllAsync())
            return NoContent();

        return BadRequest("problem setting the main photo");
    }
    
}
