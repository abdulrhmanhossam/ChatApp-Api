using ChatApp.API.Entities;
using ChatApp.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.Data.Repositories;
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<AppUser> GetUserByIdAsync(int id)
    {
        return await _dbContext.Users.FindAsync(id);
    }

    public async Task<AppUser> GetUserByUsernameAsync(string username)
    {
        return await _dbContext.Users.SingleOrDefaultAsync(x => x.UserName == username);
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await _dbContext.Users.ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public void Update(AppUser user)
    {
        _dbContext.Entry(user).State = EntityState.Modified;
    }
}