namespace SocialNetwork.Application.Models;

public record Message(long Id, UserPreview? Sender, string Content, DateTime SentAt);