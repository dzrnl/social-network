namespace SocialNetwork.Presentation.Web.Models.Friends;

public record GetUserFriendsModel(long UserId, int Page = 1, int PageSize = 10);