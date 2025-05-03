using SocialNetwork.Application.Models;

namespace SocialNetwork.Presentation.Web.Models.Users;

public record UserPreviewModel(long Id, string Username, string Name, string Surname)
{
    public static UserPreviewModel ToViewModel(UserPreview user)
    {
        return new UserPreviewModel(user.Id, user.Username, user.Name, user.Surname);
    }
}