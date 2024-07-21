using ChatApp.API.Interfaces;

namespace ChatApp.API.Data;

public class UnitOfWork(AppDbContext dbContext, IUserRepository userRepository,
    ILikesRepository likesRepository, IMessageRepository messageRepository) : IUnitOfWork
{
    public IUserRepository UserRepository => userRepository;

    public IMessageRepository MessageRepository => messageRepository;

    public ILikesRepository LikesRepository => likesRepository;

    public async Task<bool> Complete()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return dbContext.ChangeTracker.HasChanges();
    }
}
