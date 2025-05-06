namespace SocialNetwork.Application.Models;

public record Post(long Id, UserPreview? Author, string Content, DateTime PublishedAt);