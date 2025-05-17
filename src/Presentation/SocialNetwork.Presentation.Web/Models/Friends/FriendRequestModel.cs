using SocialNetwork.Application.Models;
using SocialNetwork.Presentation.Web.Models.Users;

namespace SocialNetwork.Presentation.Web.Models.Friends;

public record FriendRequestModel(long Id, UserPreviewModel FromUser, UserPreviewModel ToUser, DateTime CreatedAt)
{
    public static FriendRequestModel ToViewModel(FriendRequest outgoingFriendRequest)
    {
        return new FriendRequestModel(
            outgoingFriendRequest.Id,
            UserPreviewModel.ToViewModel(outgoingFriendRequest.FromUser),
            UserPreviewModel.ToViewModel(outgoingFriendRequest.ToUser),
            outgoingFriendRequest.CreatedAt);
    }
}