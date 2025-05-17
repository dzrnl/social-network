using SocialNetwork.Application.Abstractions.Queries;
using SocialNetwork.Application.Abstractions.Repositories;
using SocialNetwork.Application.Contracts.Commands.Friends;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Services;

public class FriendshipService : IFriendshipService
{
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IFriendRequestRepository _friendRequestRepository;
    private readonly IUserRepository _userRepository;

    public FriendshipService(
        IFriendshipRepository friendshipRepository,
        IFriendRequestRepository friendRequestRepository,
        IUserRepository userRepository)
    {
        _friendshipRepository = friendshipRepository;
        _friendRequestRepository = friendRequestRepository;
        _userRepository = userRepository;
    }

    public async Task<SendFriendRequestCommand.Response> SendFriendRequest(SendFriendRequestCommand.Request request)
    {
        if (request.UserId == request.FriendId)
        {
            return new SendFriendRequestCommand.Response.SelfFriendship();
        }

        try
        {
            var bothUsersExist = await CheckBothUsersExist(request.UserId, request.FriendId);

            if (!bothUsersExist)
            {
                return new SendFriendRequestCommand.Response.UserNotFound();
            }

            var alreadyFriends = await _friendshipRepository.AreFriends(request.UserId, request.FriendId);

            if (alreadyFriends)
            {
                return new SendFriendRequestCommand.Response.AlreadyFriends();
            }

            var alreadySentRequest = await _friendRequestRepository.ExistsByUsers(request.UserId, request.FriendId);

            if (alreadySentRequest)
            {
                return new SendFriendRequestCommand.Response.RequestAlreadySent();
            }

            var friendRequest = await _friendRequestRepository.Add(request.UserId, request.FriendId);

            return new SendFriendRequestCommand.Response.Success(friendRequest);
        }
        catch (Exception)
        {
            return new SendFriendRequestCommand.Response.Failure("Unexpected error while adding friend");
        }
    }

    public async Task<GetFriendRequestCommand.Response> GetFriendRequestById(GetFriendRequestCommand.Request.ById request)
    {
        try
        {
            var friendRequest = await _friendRequestRepository.FindById(request.Id);

            if (friendRequest == null)
            {
                return new GetFriendRequestCommand.Response.NotFound();
            }

            return new GetFriendRequestCommand.Response.Success(friendRequest);
        }
        catch (Exception)
        {
            return new GetFriendRequestCommand.Response.Failure("Unexpected error while getting friend request.");
        }
    }
    
    public async Task<GetFriendRequestCommand.Response> GetFriendRequestByUsers(GetFriendRequestCommand.Request.ByUsers request)
    {
        try
        {
            var bothUsersExist = await CheckBothUsersExist(request.UserId1, request.UserId2);

            if (!bothUsersExist)
            {
                return new GetFriendRequestCommand.Response.UserNotFound();
            }
            
            var friendRequest = await _friendRequestRepository.FindByUserIds(request.UserId1, request.UserId2);

            if (friendRequest == null)
            {
                return new GetFriendRequestCommand.Response.NotFound();
            }

            return new GetFriendRequestCommand.Response.Success(friendRequest);
        }
        catch (Exception)
        {
            return new GetFriendRequestCommand.Response.Failure("Unexpected error while getting friend request.");
        }
    }

    public async Task<AcceptFriendRequestCommand.Response> AcceptFriendRequest(AcceptFriendRequestCommand.Request request)
    {
        try
        {
            var friendRequest = await _friendRequestRepository.FindById(request.RequestId);

            if (friendRequest == null)
            {
                return new AcceptFriendRequestCommand.Response.NoPendingRequest();
            }

            await _friendshipRepository.AddFriend(friendRequest.FromUser.Id, friendRequest.ToUser.Id);

            await _friendRequestRepository.Delete(friendRequest.Id);

            return new AcceptFriendRequestCommand.Response.Success();
        }
        catch (Exception)
        {
            return new AcceptFriendRequestCommand.Response.Failure("Unexpected error while declining friend request.");
        }
    }

    public async Task<DeclineFriendRequestCommand.Response> DeclineFriendRequest(DeclineFriendRequestCommand.Request request)
    {
        try
        {
            var friendRequest = await _friendRequestRepository.FindById(request.RequestId);

            if (friendRequest == null)
            {
                return new DeclineFriendRequestCommand.Response.NoPendingRequest();
            }

            await _friendRequestRepository.Delete(friendRequest.Id);

            return new DeclineFriendRequestCommand.Response.Success();
        }
        catch (Exception)
        {
            return new DeclineFriendRequestCommand.Response.Failure("Unexpected error while declining friend request.");
        }
    }

    public async Task<CancelFriendRequestCommand.Response> CancelFriendRequest(CancelFriendRequestCommand.Request request)
    {
        try
        {
            var friendRequest = await _friendRequestRepository.FindById(request.RequestId);

            if (friendRequest == null)
            {
                return new CancelFriendRequestCommand.Response.NoPendingRequest();
            }

            await _friendRequestRepository.Delete(friendRequest.Id);

            return new CancelFriendRequestCommand.Response.Success();
        }
        catch (Exception)
        {
            return new CancelFriendRequestCommand.Response.Failure("Unexpected error while canceling friend request.");
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

    public async Task<GetFriendStatusCommand.Response> GetFriendStatus(GetFriendStatusCommand.Request request)
    {
        try
        {
            var bothUsersExist = await CheckBothUsersExist(request.UsedId, request.FriendId);

            if (!bothUsersExist)
            {
                return new GetFriendStatusCommand.Response.UserNotFound();
            }

            var areFriends = await _friendshipRepository.AreFriends(request.UsedId, request.FriendId);

            if (areFriends)
            {
                return new GetFriendStatusCommand.Response.Success(FriendStatus.Friend);
            }

            var isSentRequest = await _friendRequestRepository.ExistsByUsers(request.UsedId, request.FriendId);

            if (isSentRequest)
            {
                return new GetFriendStatusCommand.Response.Success(FriendStatus.Sent);
            }
            
            var isIncomingRequest = await _friendRequestRepository.ExistsByUsers(request.FriendId, request.UsedId);

            if (isIncomingRequest)
            {
                return new GetFriendStatusCommand.Response.Success(FriendStatus.Incoming);
            }

            return new GetFriendStatusCommand.Response.Success(FriendStatus.NotFriend);
        }
        catch (Exception)
        {
            return new GetFriendStatusCommand.Response.Failure("Unexpected error while getting friend status");
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

    public async Task<GetUserIncomingRequestsCommand.Response> GetUserIncomingRequests(GetUserIncomingRequestsCommand.Request request)
    {
        try
        {
            var userExists = await _userRepository.ExistsById(request.UsedId);

            if (!userExists)
            {
                return new GetUserIncomingRequestsCommand.Response.UserNotFound();
            }

            var requests = await _friendRequestRepository.GetIncomingRequests(request.UsedId);

            return new GetUserIncomingRequestsCommand.Response.Success(requests);
        }
        catch (Exception)
        {
            return new GetUserIncomingRequestsCommand.Response.Failure("Unexpected error while getting incoming requests");
        }
    }

    public async Task<GetUserSentRequestsCommand.Response> GetUserSentRequests(GetUserSentRequestsCommand.Request request)
    {
        try
        {
            var userExists = await _userRepository.ExistsById(request.UsedId);

            if (!userExists)
            {
                return new GetUserSentRequestsCommand.Response.UserNotFound();
            }

            var requests = await _friendRequestRepository.GetSentRequests(request.UsedId);

            return new GetUserSentRequestsCommand.Response.Success(requests);
        }
        catch (Exception)
        {
            return new GetUserSentRequestsCommand.Response.Failure("Unexpected error while getting sent requests");
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