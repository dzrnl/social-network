using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Presentation.Web.Models.Auth;

public record LoginModel
{
    [Required(ErrorMessage = "Username is required")]
    public required string Username { get; init; }

    [Required(ErrorMessage = "Password is required")]
    public required string Password { get; init; }
}