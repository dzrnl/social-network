namespace SocialNetwork.Infrastructure.DataAccess.Entities;

public class FriendRequestEntity
{
    public long Id { get; set; }

    public long FromUserId { get; set; }
    public required UserEntity FromUser { get; set; }

    public long ToUserId { get; set; }
    public required UserEntity ToUser { get; set; }

    public DateTime CreatedAt { get; set; }
}