using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Presentation.Web.Models.Users;

public record ChangeUserSurnameModel
{
    [Required(ErrorMessage = "Surname is required")]
    [MaxLength(255, ErrorMessage = "Surname is too long")]
    public required string Surname { get; init; }
}