namespace SocialNetwork.Infrastructure.DataAccess.Entities;

public class DirectChatEntity
{
    public long Id { get; set; }

    public long? User1Id { get; set; }
    public UserEntity? User1 { get; set; }
    
    public long? User2Id { get; set; }
    public UserEntity? User2 { get; set; }

    public long? LastMessageId { get; set; }
    public MessageEntity? LastMessage { get; set; }

    public IList<MessageEntity> Messages { get; set; } = [];
}