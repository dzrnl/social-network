using SocialNetwork.Application.Models;

namespace SocialNetwork.Presentation.Web.Models.Users;

public record UserModel(long Id, string Username, string Name)
{
    public static UserModel ToViewModel(User user)
    {
        return new UserModel(user.Id, user.Username, user.Name);
    }
}