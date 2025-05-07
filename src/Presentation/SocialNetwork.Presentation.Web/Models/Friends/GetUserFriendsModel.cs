using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Presentation.Web.Models.Friends;

public record GetUserFriendsModel([Required] long UserId) : PaginatedRequest;