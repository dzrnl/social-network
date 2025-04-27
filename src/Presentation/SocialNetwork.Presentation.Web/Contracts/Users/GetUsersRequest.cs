namespace SocialNetwork.Presentation.Web.Contracts.Users;

public record GetUsersRequest(int Page = 1, int PageSize = 10);