using ChatApp.API.Data;
using ChatApp.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ChatApp.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly AppDbContext _dbContext;
        public AccountController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto) 
        {
            if (await UserExists(registerDto.Username))
                return BadRequest("Username is taken");

            using var hmac = new HMACSHA512();

            var user = new AppUser()
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)), // make password as hash byte
                PasswordSalt = hmac.Key // add random key
            };
            
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        // to check if this user name is taken or not
        private async Task<bool> UserExists(string userName)
        {
            return await _dbContext.Users.
                AnyAsync(u => u.UserName == userName.ToLower());
        }
    }
}
