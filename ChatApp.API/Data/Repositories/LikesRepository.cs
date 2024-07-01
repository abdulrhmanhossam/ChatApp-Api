using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChatApp.API.DTOs;
using ChatApp.API.Entities;
using ChatApp.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.Data.Repositories;
public class LikesRepository(AppDbContext dbContext, IMapper mapper) : ILikesRepository
{
    public void AddLike(UserLike like)
    {
        dbContext.Likes.Add(like);
    }

    public void DeleteLike(UserLike like)
    {
        dbContext.Likes.Remove(like);
    }

    // return every one the user give a like 
    public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
    {
        return await dbContext.Likes
            .Where(u => u.SourceUserId == currentUserId)
            .Select(x => x.TargetUserId)
            .ToListAsync();
    }

    // return user likes 
    public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await dbContext.Likes
            .FindAsync(sourceUserId, targetUserId);
    }

    // return by constrain  
    public async Task<IEnumerable<MemberDto>> GetUserLikes(string predicate, int userId)
    {
        var likes = dbContext.Likes.AsQueryable();

        switch (predicate)
        {
            case "liked":
                return await likes
                    .Where(u => u.SourceUserId == userId)
                    .Select(x => x.TargetUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                    .ToListAsync();

            case "likedBy":
                return await likes
                    .Where(u => u.TargetUserId == userId)
                    .Select(x => x.SourceUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                    .ToListAsync();

            default:
                var likeIds = await GetCurrentUserLikeIds(userId);

                return await likes
                    .Where(u => u.TargetUserId == userId && likeIds
                    .Contains(u.SourceUserId))
                    .Select(x => x.SourceUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
                    .ToListAsync();
        }
    }

    public async Task<bool> SaveChanges()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }
}
