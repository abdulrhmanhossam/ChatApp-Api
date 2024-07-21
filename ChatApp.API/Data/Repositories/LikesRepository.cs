using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChatApp.API.DTOs;
using ChatApp.API.Entities;
using ChatApp.API.Helpers;
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
    public async Task<PagedList<MemberDto>> GetUserLikes(LikesParams likesParams)
    {
        var likes = dbContext.Likes.AsQueryable();

        IQueryable<MemberDto> query;

        switch (likesParams.Predicate)
        {
            case "liked":
                query = likes
                    .Where(u => u.SourceUserId == likesParams.UserId)
                    .Select(x => x.TargetUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;

            case "likedBy":
                query = likes
                    .Where(u => u.TargetUserId == likesParams.UserId)
                    .Select(x => x.SourceUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                break;

            default:
                var likeIds = await GetCurrentUserLikeIds(likesParams.UserId);

                query = likes
                    .Where(u => u.TargetUserId == likesParams.UserId && likeIds
                    .Contains(u.SourceUserId))
                    .Select(x => x.SourceUser)
                    .ProjectTo<MemberDto>(mapper.ConfigurationProvider);
                
                break;
        }

        return await PagedList<MemberDto>
            .CreateAsync(query, likesParams.PageNumber, likesParams.PageSize);
    }
}
