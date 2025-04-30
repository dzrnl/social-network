using SocialNetwork.Application.Contracts.Commands.Friends;

namespace SocialNetwork.Application.Contracts.Services;

public interface IFriendshipService
{
    Task<AddFriendCommand.Response> AddFriend(AddFriendCommand.Request request);

    Task<RemoveFriendCommand.Response> RemoveFriend(RemoveFriendCommand.Request request);

    Task<AreFriendsCommand.Response> AreFriends(AreFriendsCommand.Request request);

    Task<GetUserFriendsCommand.Response> GetUserFriends(GetUserFriendsCommand.Request request);
}