using ChatApp.API.DTOs;
using ChatApp.API.Entities;

namespace ChatApp.API.Interfaces;
public interface IUserRepository
{
    void Update(AppUser user);
    Task<bool> SaveAllAsync();
    // Task<IEnumerable<AppUser>> GetUsersAsync();
    // Task<AppUser> GetUserByIdAsync(int id);
    // Task<AppUser> GetUserByUsernameAsync(string username);
    Task<IEnumerable<MemberDto>> GetMembersAsync();
    Task<MemberDto> GetMemberByNameAsync(string username);
    Task<MemberDto> GetMemberByIdAsync(int id);

}