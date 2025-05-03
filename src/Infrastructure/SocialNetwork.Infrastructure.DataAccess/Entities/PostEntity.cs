namespace SocialNetwork.Infrastructure.DataAccess.Entities;

public class PostEntity
{
    public long Id { get; set; }

    public long? AuthorId { get; set; }
    public UserEntity? Author { get; set; }

    public required string Content { get; set; }

    public DateTime PublishedAt { get; set; }
}