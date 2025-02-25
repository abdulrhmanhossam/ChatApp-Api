# ChatApp API - Real-Time Chat Application Backend

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-blue)](https://dotnet.microsoft.com/)
[![SignalR](https://img.shields.io/badge/SignalR-8.0-green)](https://learn.microsoft.com/en-us/aspnet/core/signalr/)
[![Entity Framework Core](https://img.shields.io/badge/EF_Core-8.0-red)](https://learn.microsoft.com/en-us/ef/core/)
[![Cloudinary](https://img.shields.io/badge/Cloudinary-3.x-blue)](https://cloudinary.com/)

A feature-rich backend API for real-time chat applications with media management capabilities.

## Features ✨
- **Real-time messaging** using SignalR
- **Media upload** to Cloudinary storage
- RESTful API endpoints with Swagger documentation
- JWT-based user authentication
- Message history storage with SQL Server
- Repository pattern implementation
- Dependency Injection integration

## Technologies 🛠️
- **ASP.NET Core 8**: Web framework
- **SignalR**: Real-time communication
- **Entity Framework Core**: ORM for database operations
- **SQL Server**: Relational database
- **Cloudinary**: Cloud media management
- **JWT**: Secure authentication
- **Swagger**: API documentation

## Design Patterns 🧩
### Repository Pattern
```csharp
public interface IMessageRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message> GetMessage(int id);
    Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
    Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);
    void AddGroup(Group group);
    void RemoveConnection(Connection connection);
    Task<Connection> GetConnection(string connectionId);
    Task<Group> GetMessageGroup(string groupName);
    Task<Group> GetGroupForConnection(string connectionId);
}
public class MessageRepository(AppDbContext dbContext, IMapper mapper) : IMessageRepository
{
    public void AddGroup(Group group)
    {
        dbContext.Groups.Add(group);
    }

    public void AddMessage(Message message)
    {
        dbContext.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        dbContext.Messages.Remove(message);
    }

    public async Task<Connection> GetConnection(string connectionId)
    {
        return await dbContext.Connections
            .FindAsync(connectionId);
    }

    public async Task<Group> GetGroupForConnection(string connectionId)
    {
        return await dbContext.Groups
            .Include(x => x.Connections)
            .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
    }

    public async Task<Message> GetMessage(int id)
    {
        return await dbContext.Messages.FindAsync(id);
    }

    public async Task<Group> GetMessageGroup(string groupName)
    {
        return await dbContext.Groups
            .Include(x => x.Connections)
            .FirstOrDefaultAsync(x => x.Name == groupName);
    }

    public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
    {
        var query = dbContext.Messages
            .OrderByDescending(x => x.MessageSent)
            .AsQueryable();

        query = messageParams.Container switch
        {
            "Inbox" => query.Where(x => x.Recipient.UserName == messageParams.Username 
                && x.RecipientDeleted == false),
            "Outbox" => query.Where(x => x.Sender.UserName == messageParams.Username 
                && x.SenderDeleted == false),
            _ => query.Where(x => x.Recipient.UserName == messageParams.Username  
                && x.DateRead == null && x.RecipientDeleted == false)
        };

        var messages = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);

        return await PagedList<MessageDto>
            .CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
    {
        var query = dbContext.Messages
            .Where(x => x.RecipientUsername == currentUsername
            && x.RecipientDeleted == false
            && x.SenderUsername == recipientUsername
            || x.SenderUsername == currentUsername
            && x.SenderDeleted == false
            && x.RecipientUsername == recipientUsername)
            .OrderBy(x => x.MessageSent)
            .AsQueryable();

        var unreadMessages = query
            .Where(x => x.DateRead == null && x.RecipientUsername == currentUsername)
            .ToList();

        if (unreadMessages.Count != 0)
        {
            unreadMessages.ForEach(x => x.DateRead = DateTime.UtcNow);
        }

        return await query
            .ProjectTo<MessageDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public void RemoveConnection(Connection connection)
    {
        dbContext.Connections.Remove(connection);
    }
}
