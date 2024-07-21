namespace ChatApp.API.SignalR;

public class PresenceTracker
{
    private static readonly Dictionary<string, List<string>> OnlineUsers = [];

    public Task UserConnected(string username, string connectionId)
    {
        lock (OnlineUsers)
        {
            if (OnlineUsers.ContainsKey(username))
                OnlineUsers[username].Add(connectionId);
            else
                OnlineUsers.Add(username, [connectionId]);
        }

        return Task.CompletedTask;
    }

    public Task UserDisconnected(string username, string connectionId)
    {
        lock (OnlineUsers)
        {
            if (!OnlineUsers.ContainsKey(username))
                return Task.CompletedTask;
            
            OnlineUsers[username].Remove(connectionId);

            if (OnlineUsers[username].Count == 0)
                OnlineUsers.Remove(username);
        }

        return Task.CompletedTask;
    }

    public Task<string[]> GetOnlineUsers()
    {
        string[] onlineUsers;

        lock (OnlineUsers)
        {
            onlineUsers = OnlineUsers
                .OrderBy(x => x.Key)
                .Select(x => x.Key)
                .ToArray();
        }

        return Task.FromResult(onlineUsers);
    }

    public static Task<List<string>> GetConnectionsForUser(string username)
    {
        List<string> connectionIds;
        if (OnlineUsers.TryGetValue(username, out var connections))
        {
            lock(connections)
            {
                connectionIds = [.. connections]; // => connections.ToList()
            }
        }
        else
        {
            connectionIds = [];
        }

        return Task.FromResult(connectionIds);
    }
}
