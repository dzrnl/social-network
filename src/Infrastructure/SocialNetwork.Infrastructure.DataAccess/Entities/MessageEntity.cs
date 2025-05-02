namespace SocialNetwork.Infrastructure.DataAccess.Entities;

public class MessageEntity
{
    public long Id { get; set; }

    public long ChatId { get; set; }
    public required DirectChatEntity Chat { get; set; }

    public long? SenderId { get; set; }
    public UserEntity? Sender { get; set; }

    public required string Content { get; set; }

    public DateTime SentAt { get; set; }
}