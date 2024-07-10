using ChatApp.API.Entities;

namespace ChatApp.API.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}
