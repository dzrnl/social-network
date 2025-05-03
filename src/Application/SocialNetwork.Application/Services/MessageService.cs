using SocialNetwork.Application.Abstractions.Queries.Messages;
using SocialNetwork.Application.Abstractions.Repositories;
using SocialNetwork.Application.Contracts.Commands.Messages;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Validations;

namespace SocialNetwork.Application.Services;

public class MessageService : IMessageService
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;

    public MessageService(IMessageRepository messageRepository, IUserRepository userRepository)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
    }

    public async Task<SendMessageCommand.Response> SendMessage(SendMessageCommand.Request request)
    {
        if (MessageValidation.ValidateContent(request.Content) is { } validationContentError)
        {
            return new SendMessageCommand.Response.InvalidRequest(validationContentError);
        }

        var query = new CreateMessageQuery(request.SenderId, request.Content);

        try
        {
            var userExists = await _userRepository.ExistsById(query.SenderId);

            if (!userExists)
            {
                return new SendMessageCommand.Response.UserNotFound();
            }

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