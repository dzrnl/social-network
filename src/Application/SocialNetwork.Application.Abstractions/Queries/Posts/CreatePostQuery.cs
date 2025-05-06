namespace SocialNetwork.Application.Abstractions.Queries.Posts;

public record CreatePostQuery(long AuthorId, string Content);