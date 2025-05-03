using SocialNetwork.Application.Abstractions.Queries.Messages;
using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Abstractions.Repositories;

public interface IMessageRepository
{
    Task<Message> Add(CreateMessageQuery query);

    Task<List<Message>> GetAll();
}