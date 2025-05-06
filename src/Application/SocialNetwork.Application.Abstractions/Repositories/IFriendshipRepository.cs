using SocialNetwork.Application.Abstractions.Queries;
using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Abstractions.Repositories;

public interface IFriendshipRepository
{
    Task<bool> AddFriend(long userId, long friendId);

    Task<bool> RemoveFriend(long userId, long friendId);

    Task<bool> AreFriends(long userId1, long userId2);

    Task<List<User>> FindFriends(long userId, PaginationQuery pagination);
}