using SocialNetwork.Application.Abstractions.Queries;
using SocialNetwork.Application.Abstractions.Repositories;
using SocialNetwork.Application.Contracts.Commands.Friends;
using SocialNetwork.Application.Contracts.Services;

namespace SocialNetwork.Application.Services;

public class FriendshipService : IFriendshipService
{
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IUserRepository _userRepository;

    public FriendshipService(IFriendshipRepository friendshipRepository, IUserRepository userRepository)
    {
        _friendshipRepository = friendshipRepository;
        _userRepository = userRepository;
    }

    public async Task<AddFriendCommand.Response> AddFriend(AddFriendCommand.Request request)
    {
        if (request.UserId == request.FriendId)
        {
            return new AddFriendCommand.Response.SelfFriendship();
        }

        try
        {
            var bothUsersExist = await CheckBothUsersExist(request.UserId, request.FriendId);

            if (!bothUsersExist)
            {
                return new AddFriendCommand.Response.UserNotFound();
            }

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
            var bothUsersExist = await CheckBothUsersExist(request.UserId, request.FriendId);

            if (!bothUsersExist)
            {
                return new RemoveFriendCommand.Response.UserNotFound();
            }

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
            var bothUsersExist = await CheckBothUsersExist(request.UserId, request.FriendId);

            if (!bothUsersExist)
            {
                return new AreFriendsCommand.Response.UserNotFound();
            }

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
        var paginationQuery = new PaginationQuery(request.Page, request.PageSize);

        try
        {
            var userExists = await _userRepository.ExistsById(request.UsedId);

            if (!userExists)
            {
                return new GetUserFriendsCommand.Response.UserNotFound();
            }

            var users = await _friendshipRepository.FindFriends(request.UsedId, paginationQuery);

            return new GetUserFriendsCommand.Response.Success(users);
        }
        catch (Exception)
        {
            return new GetUserFriendsCommand.Response.Failure("Unexpected error while getting friends");
        }
    }

    private async Task<bool> CheckBothUsersExist(long userId1, long userId2)
    {
        var user1Exists = await _userRepository.ExistsById(userId1);

        if (!user1Exists)
        {
            return false;
        }

        var user2Exists = await _userRepository.ExistsById(userId2);

        return user2Exists;
    }
}