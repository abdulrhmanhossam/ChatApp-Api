using ChatApp.API.DTOs;
using ChatApp.API.Entities;
using ChatApp.API.Extensions;
using ChatApp.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers;
public class LikesController(ILikesRepository likesRepository) : BaseApiController
{
    [HttpPost("{targetUserId:int}")]
    public async Task<ActionResult> ToggleLike(int targetUserId)
    {
        var sourceUserId = User.GetUserId();

        if (sourceUserId == targetUserId) 
            return BadRequest("You cannot like your self");
        
        var existingLike = await likesRepository.GetUserLike(sourceUserId, targetUserId);

        if (existingLike == null)
        {
            var like = new UserLike
            {
                SourceUserId = sourceUserId,
                TargetUserId = targetUserId
            };

            likesRepository.AddLike(like);
        }
        else
        {
            likesRepository.DeleteLike(existingLike);
        }

        if (await likesRepository.SaveChanges())
            return Ok();

        return BadRequest("Failed to update like");
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikesIds()
    {
        return Ok(await likesRepository.GetCurrentUserLikeIds(User.GetUserId()));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserLikes(string predicate)
    {
        return Ok(await likesRepository.GetUserLikes(predicate, User.GetUserId()));
    }
}
