using ChatApp.API.DTOs;
using ChatApp.API.Entities;
using ChatApp.API.Helpers;
using ChatApp.API.Interfaces;

namespace ChatApp.API.Data.Repositories;
public class MessageRepository(AppDbContext dbContext) : IMessageRepository
{
    public void AddMessage(Message message)
    {
        dbContext.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        dbContext.Messages.Remove(message);
    }

    public async Task<Message> GetMessage(int id)
    {
        return await dbContext.Messages.FindAsync(id);
    }

    public async Task<PagedList<MessageDto>> GetMessagesForUser()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }
}
