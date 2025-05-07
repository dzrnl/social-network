namespace SocialNetwork.Presentation.Web.Models.Users;

public record SearchUsersResultModel(int Page, int PageSize, List<UserModel> Users, string? Query);