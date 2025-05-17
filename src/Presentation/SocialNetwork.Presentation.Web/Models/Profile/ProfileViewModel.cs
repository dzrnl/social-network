using SocialNetwork.Application.Models;
using SocialNetwork.Presentation.Web.Models.Friends;
using SocialNetwork.Presentation.Web.Models.Posts;
using SocialNetwork.Presentation.Web.Models.Users;

namespace SocialNetwork.Presentation.Web.Models.Profile;

public record ProfileViewModel(UserModel User, FriendStatus? FriendStatus, FriendRequestModel? FriendRequest, List<PostModel> UserPosts);