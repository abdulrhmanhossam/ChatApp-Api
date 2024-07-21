using ChatApp.API.DTOs;
using ChatApp.API.Entities;
using ChatApp.API.Helpers;

namespace ChatApp.API.Interfaces;
public interface IUserRepository
{
    void Update(AppUser user);
    Task<IEnumerable<AppUser>> GetUsersAsync();
    Task<AppUser> GetUserByIdAsync(int id);
    Task<AppUser> GetUserByUsernameAsync(string username);
    Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
    Task<MemberDto> GetMemberByNameAsync(string username);
    Task<MemberDto> GetMemberByIdAsync(int id);

}