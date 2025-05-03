namespace SocialNetwork.Application.Abstractions.Queries.Messages;

public record CreateMessageQuery(long SenderId, string Content);