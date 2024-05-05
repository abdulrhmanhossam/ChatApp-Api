using AutoMapper;
using ChatApp.API.DTOs;
using ChatApp.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
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
}
