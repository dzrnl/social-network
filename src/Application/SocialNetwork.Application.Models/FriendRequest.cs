namespace SocialNetwork.Application.Models;

public record FriendRequest(long Id, UserPreview FromUser, UserPreview ToUser, DateTime CreatedAt);