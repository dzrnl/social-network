using Microsoft.AspNetCore.SignalR;
using SocialNetwork.Application.Contracts.Commands.Messages;
using SocialNetwork.Application.Contracts.Services;

namespace SocialNetwork.Presentation.Web.Hubs;

public class ChatHub : Hub
{
    private readonly IMessageService _messageService;

    public ChatHub(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public async Task SendMessage(long userId, string content)
    {
        var response = await _messageService.SendMessage(new(userId, content));

        if (response is SendMessageCommand.Response.Failure)
        {
            return;
        }

        var success = (SendMessageCommand.Response.Success)response;

        var message = success.Message;

        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}