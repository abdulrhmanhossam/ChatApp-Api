using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChatApp.API.DTOs;
using ChatApp.API.Entities;
using ChatApp.API.Helpers;
using ChatApp.API.Interfaces;

namespace ChatApp.API.Data.Repositories;
public class MessageRepository(AppDbContext dbContext, IMapper mapper) : IMessageRepository
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

    public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
    {
        var query = dbContext.Messages
            .OrderByDescending(x => x.MessageSent)
            .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(x => x.Recipient.UserName == messageParams.Username),
            "Outbox" => query.Where(x => x.Sender.UserName == messageParams.Username),
            _ => query.Where(x => x.Recipient.UserName == messageParams.Username  
                && x.DateRead == null)
        };

        var messages = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);

        return await PagedList<MessageDto>
            .CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
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
