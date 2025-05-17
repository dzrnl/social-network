using SocialNetwork.Application.Contracts.Commands.Friends;

namespace SocialNetwork.Application.Contracts.Services;

public interface IFriendshipService
{
    Task<SendFriendRequestCommand.Response> SendFriendRequest(SendFriendRequestCommand.Request request);

    Task<GetFriendRequestCommand.Response> GetFriendRequestById(GetFriendRequestCommand.Request.ById request);
    
    Task<GetFriendRequestCommand.Response> GetFriendRequestByUsers(GetFriendRequestCommand.Request.ByUsers request);
    
    Task<AcceptFriendRequestCommand.Response> AcceptFriendRequest(AcceptFriendRequestCommand.Request request);

    Task<DeclineFriendRequestCommand.Response> DeclineFriendRequest(DeclineFriendRequestCommand.Request request);

    Task<CancelFriendRequestCommand.Response> CancelFriendRequest(CancelFriendRequestCommand.Request request);

    Task<RemoveFriendCommand.Response> RemoveFriend(RemoveFriendCommand.Request request);

    Task<AreFriendsCommand.Response> AreFriends(AreFriendsCommand.Request request);
    
    Task<GetFriendStatusCommand.Response> GetFriendStatus(GetFriendStatusCommand.Request request);

    Task<GetUserFriendsCommand.Response> GetUserFriends(GetUserFriendsCommand.Request request);

    Task<GetUserIncomingRequestsCommand.Response> GetUserIncomingRequests(GetUserIncomingRequestsCommand.Request request);

    Task<GetUserSentRequestsCommand.Response> GetUserSentRequests(GetUserSentRequestsCommand.Request request);
}