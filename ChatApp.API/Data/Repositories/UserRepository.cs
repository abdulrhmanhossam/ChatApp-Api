using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChatApp.API.DTOs;
using ChatApp.API.Entities;
using ChatApp.API.Helpers;
using ChatApp.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.Data.Repositories;
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public UserRepository(AppDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<MemberDto> GetMemberByIdAsync(int id)
    {
        return await _dbContext.Users
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<MemberDto> GetMemberByNameAsync(string username)
    {
        return await _dbContext.Users
            .Where(x => x.UserName == username)
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
    }

    public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
    {
       var query = _dbContext.Users.AsQueryable();

       query = query.Where(x => x.UserName != userParams.CurrentUserName);

       if (userParams.Gender != null)
           query = query.Where(x => x.Gender == userParams.Gender);

        var minDob = DateOnly
            .FromDateTime(DateTime.Today
            .AddYears(-userParams.MaxAge - 1));

        var maxDob = DateOnly
            .FromDateTime(DateTime.Today
            .AddYears(-userParams.MinAge));

        query = query
            .Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

        query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(x => x.Created),
            _ => query.OrderByDescending(x => x.LastActive)
        };

        return await PagedList<MemberDto>
            .CreateAsync(query
            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
            .AsNoTracking(), userParams.PageNumber, userParams.PageSize);
    }
    
    public async Task<AppUser> GetUserByIdAsync(int id)
    {
        return await _dbContext.Users
            .Include(p => p.Photos)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<AppUser> GetUserByUsernameAsync(string username)
    {
        return await _dbContext.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await _dbContext.Users
            .Include(p => p.Photos)
            .ToListAsync();
    }

    public void Update(AppUser user)
    {
        _dbContext.Entry(user).State = EntityState.Modified;
    }
}