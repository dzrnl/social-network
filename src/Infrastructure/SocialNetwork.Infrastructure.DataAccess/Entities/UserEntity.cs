namespace SocialNetwork.Infrastructure.DataAccess.Entities;

public class UserEntity
{
    public long Id { get; set; }

    public required string Name { get; set; }
}