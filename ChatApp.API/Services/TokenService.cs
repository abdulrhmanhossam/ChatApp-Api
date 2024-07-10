using ChatApp.API.Entities;
using ChatApp.API.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatApp.API.Services
{
    public class TokenService : ITokenService
    {
        //private readonly IConfiguration _configuration;

        // create key and configruation to this key
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration configuration)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"]));
        }

        public string CreateToken(AppUser user)
        {
            if (user.UserName == null) 
                throw new ArgumentNullException("user");

            // create claims by adding claim type & value
            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new (ClaimTypes.Name, user.UserName)
            };

            // create credential and this will be signature of token 
            var credential = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // add componant to tokenDescriptor to generate token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = credential
            };

            // use token handler to handle token create and write
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
