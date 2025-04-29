namespace SocialNetwork.Presentation.Web.Models.Users;

public record SearchUsersModel(int Page, int PageSize, List<UserModel> Users);