using ChatApp.API.DTOs;
using ChatApp.API.Entities;
using ChatApp.API.Helpers;

namespace ChatApp.API.Interfaces;
public interface ILikesRepository
{
    Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);
    Task<PagedList<MemberDto>> GetUserLikes(LikesParams likesParams);
    Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId);
    void DeleteLike(UserLike like);
    void AddLike(UserLike like);
}