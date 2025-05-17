using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Abstractions.Repositories;

public interface IFriendRequestRepository
{
    Task<FriendRequest> Add(long fromUserId, long toUserId);

    Task<FriendRequest?> FindById(long id);
    
    Task<FriendRequest?> FindByUserIds(long userId1, long userId2);

    Task<bool> ExistsByUsers(long fromUserId, long toUserId);

    Task<bool> Delete(long id);

    Task<List<FriendRequest>> GetIncomingRequests(long userId);

    Task<List<FriendRequest>> GetSentRequests(long userId);
}