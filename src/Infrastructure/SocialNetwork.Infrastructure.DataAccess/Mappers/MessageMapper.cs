using SocialNetwork.Application.Models;
using SocialNetwork.Infrastructure.DataAccess.Entities;

namespace SocialNetwork.Infrastructure.DataAccess.Mappers;

public static class MessageMapper
{
    public static Message ToDomain(this MessageEntity messageEntity)
    {
        return new Message(
            messageEntity.Id, 
            messageEntity.Sender?.ToPreview(), 
            messageEntity.Content, 
            messageEntity.SentAt);
    }
}