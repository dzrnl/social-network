using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Presentation.Web.Models.Users;

public record ChangeUserNameModel
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(255, ErrorMessage = "Name is too long")]
    public required string Name { get; init; }
}