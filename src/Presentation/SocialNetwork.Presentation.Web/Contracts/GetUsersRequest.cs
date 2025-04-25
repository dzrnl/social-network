namespace SocialNetwork.Presentation.Web.Contracts;

public record GetUsersRequest(int Page = 1, int PageSize = 10);