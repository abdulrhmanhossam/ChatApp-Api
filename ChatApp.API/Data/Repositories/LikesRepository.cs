using ChatApp.API.DTOs;
using ChatApp.API.Entities;
using ChatApp.API.Interfaces;

namespace ChatApp.API.Data.Repositories;
public class LikesRepository : ILikesRepository
{
    public void AddLike(UserLike like)
    {
        throw new NotImplementedException();
    }

    public void DeleteLike(UserLike like)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
    {
        throw new NotImplementedException();
    }

    public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<MemberDto>> GetUserLikes(string predicate, int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SaveChanges()
    {
        throw new NotImplementedException();
    }
}
