using SocialNetwork.Application.Models;

namespace SocialNetwork.Presentation.Web.Models.Users;

public record UserResponse(long Id, string Username, string Name)
{
    public static UserResponse ToResponse(User user)
    {
        return new UserResponse(user.Id, user.Username, user.Name);
    }
}