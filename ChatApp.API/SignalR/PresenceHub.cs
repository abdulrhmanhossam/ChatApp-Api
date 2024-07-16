using ChatApp.API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.API.SignalR;

[Authorize]
public class PresenceHub(PresenceTracker tracker) : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (Context.User == null)
            throw new HubException("Cannot get current user claim");

        await tracker.UserConnected(Context.User.GetUserName(), Context.ConnectionId);
        
        await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUserName());

        var currentUsers = await tracker.GetOnlineUsers();
        await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        if (Context.User == null)
            throw new HubException("Cannot get current user claim");

        await tracker.UserDisconnected(Context.User.GetUserName(), Context.ConnectionId);

        await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserName());

        var currentUsers = await tracker.GetOnlineUsers();
        await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

        await base.OnDisconnectedAsync(exception);
    }
}

