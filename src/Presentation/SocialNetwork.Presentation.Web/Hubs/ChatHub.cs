using Microsoft.AspNetCore.SignalR;

namespace SocialNetwork.Presentation.Web.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(long userId, string username, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", userId, username, message);
    }
}