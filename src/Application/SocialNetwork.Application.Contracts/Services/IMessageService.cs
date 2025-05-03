using SocialNetwork.Application.Contracts.Commands.Messages;

namespace SocialNetwork.Application.Contracts.Services;

public interface IMessageService
{
    Task<SendMessageCommand.Response> SendMessage(SendMessageCommand.Request request);
    
    Task<GetAllMessagesCommand.Response> GetAllMessages(GetAllMessagesCommand.Request request);
}