using SocialNetwork.Application.Abstractions.Queries.Messages;
using SocialNetwork.Application.Abstractions.Repositories;
using SocialNetwork.Application.Contracts.Commands.Messages;
using SocialNetwork.Application.Contracts.Services;

namespace SocialNetwork.Application.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    
    public MessageService(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }
    
    public async Task<SendMessageCommand.Response> SendMessage(SendMessageCommand.Request request)
    {
        var query = new CreateMessageQuery(request.SenderId, request.Content);

        try
        {
            var message = await _messageRepository.Add(query);

            return new SendMessageCommand.Response.Success(message);
        }
        catch (Exception)
        {
            return new SendMessageCommand.Response.Failure("Unexpected error while sending message");
        }
    }

    public async Task<GetAllMessagesCommand.Response> GetAllMessages(GetAllMessagesCommand.Request request)
    {
        try
        {
            var messages = await _messageRepository.GetAll();

            return new GetAllMessagesCommand.Response.Success(messages);
        }
        catch (Exception)
        {
            return new GetAllMessagesCommand.Response.Failure("Unexpected error while getting all messages");
        }
    }
}