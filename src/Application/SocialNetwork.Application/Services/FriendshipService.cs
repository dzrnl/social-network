using SocialNetwork.Application.Abstractions.Repositories;
using SocialNetwork.Application.Contracts.Commands.Friends;
using SocialNetwork.Application.Contracts.Services;

namespace SocialNetwork.Application.Services;

public class FriendshipService : IFriendshipService
{
    private readonly IFriendshipRepository _friendshipRepository;

    public FriendshipService(IFriendshipRepository friendshipRepository)
    {
        _friendshipRepository = friendshipRepository;
    }

    public async Task<AddFriendCommand.Response> AddFriend(AddFriendCommand.Request request)
    {
        if (request.UserId == request.FriendId)
        {
            return new AddFriendCommand.Response.SelfFriendship();
        }

        try
        {
            var added = await _friendshipRepository.AddFriend(request.UserId, request.FriendId);

            if (!added)
            {
                return new AddFriendCommand.Response.AlreadyFriends();
            }

            return new AddFriendCommand.Response.Success();
        }
        catch (Exception)
        {
            return new AddFriendCommand.Response.Failure("Unexpected error while adding friend");
        }
    }

    public async Task<RemoveFriendCommand.Response> RemoveFriend(RemoveFriendCommand.Request request)
    {
        try
        {
            var removed = await _friendshipRepository.RemoveFriend(request.UserId, request.FriendId);

            if (!removed)
            {
                return new RemoveFriendCommand.Response.NotFriends();
            }

            return new RemoveFriendCommand.Response.Success();
        }
        catch (Exception)
        {
            return new RemoveFriendCommand.Response.Failure("Unexpected error while removing friend");
        }
    }

    public async Task<AreFriendsCommand.Response> AreFriends(AreFriendsCommand.Request request)
    {
        try
        {
            var result = await _friendshipRepository.AreFriends(request.UserId, request.FriendId);

            return new AreFriendsCommand.Response.Success(result);
        }
        catch (Exception)
        {
            return new AreFriendsCommand.Response.Failure("Unexpected error while checking friendship");
        }
    }

    public async Task<GetUserFriendsCommand.Response> GetUserFriends(GetUserFriendsCommand.Request request)
    {
        try
        {
            var users = await _friendshipRepository.FindFriends(request.UsedId);

            return new GetUserFriendsCommand.Response.Success(users);
        }
        catch (Exception)
        {
            return new GetUserFriendsCommand.Response.Failure("Unexpected error while getting friends");
        }
    }
}