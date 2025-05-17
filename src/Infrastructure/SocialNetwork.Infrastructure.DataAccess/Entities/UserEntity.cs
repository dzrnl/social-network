namespace SocialNetwork.Infrastructure.DataAccess.Entities;

public class UserEntity
{
    public long Id { get; set; }

    public required string Username { get; set; }

    public required string PasswordHash { get; set; }

    public required string Name { get; set; }

    public required string Surname { get; set; }

    public List<UserEntity> Friends { get; set; } = [];

    public List<FriendRequestEntity> SentRequests { get; set; } = [];
    public List<FriendRequestEntity> ReceivedRequests { get; set; } = [];
}