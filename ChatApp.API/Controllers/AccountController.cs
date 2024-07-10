using AutoMapper;
using ChatApp.API.DTOs;
using ChatApp.API.Entities;
using ChatApp.API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.Controllers
{
    public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService,
        IMapper mapper) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) 
        {
            if (await UserExists(registerDto.Username))
                return BadRequest("Username is taken");

            var user = mapper.Map<AppUser>(registerDto);

            user.UserName = registerDto.Username.ToLower();

            var result = await userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // return username and token
            return new UserDto
            {
                Username = user.UserName,
                Token = await tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // get user from database
            var user = await userManager.Users
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(u => u.NormalizedUserName == loginDto.Username.ToUpper());
            
            // check if user null
            if (user == null || user.UserName == null)
                return Unauthorized("Invalid Username");

            return new UserDto
            {
                Username = user.UserName,
                Token = await tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };

        }

        // to check if this username is taken or not
        private async Task<bool> UserExists(string userName)
        {
            return await userManager.Users.
                AnyAsync(u => u.NormalizedUserName == userName.ToUpper());
        }
    }
}
