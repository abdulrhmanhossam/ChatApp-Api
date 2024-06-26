﻿using AutoMapper;
using ChatApp.API.Data;
using ChatApp.API.DTOs;
using ChatApp.API.Entities;
using ChatApp.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ChatApp.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly AppDbContext _dbContext;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(AppDbContext dbContext, ITokenService tokenService, IMapper mapper)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto) 
        {
            if (await UserExists(registerDto.Username))
                return BadRequest("Username is taken");

            var user = _mapper.Map<AppUser>(registerDto);

            using var hmac = new HMACSHA512();

            user.UserName = registerDto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)); // make password as hash byte
            user.PasswordSalt = hmac.Key; // add random key
            
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // return username and token
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // get user from database
            var user = await _dbContext.Users
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(u => u.UserName == loginDto.Username);
            
            // check if user null
            if (user == null)
                return Unauthorized("Invalid Username");
            
            // create hmac and take the key as password salt 
            using var hmac = new HMACSHA512(user.PasswordSalt);

            // make password array of byte and keep it at computedHash
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            // loop on this array of byte and compare old password hash with new 
            for (var i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    return Unauthorized("Invalid Password");
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };

        }

        // to check if this username is taken or not
        private async Task<bool> UserExists(string userName)
        {
            return await _dbContext.Users.
                AnyAsync(u => u.UserName == userName.ToLower());
        }
    }
}
