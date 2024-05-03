using ChatApp.API.Data;
using ChatApp.API.Entities;
using ChatApp.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        return Ok(await _userRepository.GetUsersAsync());
    }

    [HttpGet("{username:alpha}")]
    public async Task<ActionResult<AppUser>> GetUser(string username)
    {
        return Ok(await _userRepository.GetUserByUsernameAsync(username));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppUser>> GetUser(int id)
    {
        return Ok(await _userRepository.GetUserByIdAsync(id));
    }
}
