using ChatApp.API.Data;
using ChatApp.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly AppDbContext _dbContext;
    public UsersController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        return Ok(await _dbContext.Users.ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppUser>> GetUser(int id)
    {
        return Ok(await _dbContext.Users.FindAsync(id));
    }
}
