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

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login(LoginDto loginDto)
        {
            // get user from database
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.UserName == loginDto.Username);
            
            // check if user null
            if (user == null)
                return Unauthorized("Invalid Username");
            
            // create hmac and take the key as passwordsalt 
            using var hmac = new HMACSHA512(user.PasswordSalt);

            // make password array of byte and keep it at computedHash
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            // loop on this array of byte and compare old password hash with new 
            for (var i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    return Unauthorized("Invalid Password");
            }

            return Ok(user);

        }

        // to check if this user name is taken or not
        private async Task<bool> UserExists(string userName)
        {
            return await _dbContext.Users.
                AnyAsync(u => u.UserName == userName.ToLower());
        }
    }
}
