using SocialNetwork.Application.Models;
using SocialNetwork.Infrastructure.DataAccess.Entities;

namespace SocialNetwork.Infrastructure.DataAccess.Mappers;

public static class FriendRequestMapper
{
    public static FriendRequest ToDomain(this FriendRequestEntity friendRequestEntity)
    {
        return new FriendRequest(
            friendRequestEntity.Id,
            friendRequestEntity.FromUser.ToPreview(),
            friendRequestEntity.ToUser.ToPreview(),
            friendRequestEntity.CreatedAt);
    }
}