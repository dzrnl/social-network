using SocialNetwork.Presentation.Web.Models.Posts;
using SocialNetwork.Presentation.Web.Models.Users;

namespace SocialNetwork.Presentation.Web.Models.Profile;

public record ProfileViewModel(UserModel User, bool IsFriend, List<PostModel> UserPosts);