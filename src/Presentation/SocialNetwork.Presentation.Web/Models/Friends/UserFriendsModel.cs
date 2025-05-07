using SocialNetwork.Presentation.Web.Models.Users;

namespace SocialNetwork.Presentation.Web.Models.Friends;

public record UserFriendsModel(List<UserPreviewModel> Friends);