using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Presentation.Web.Models.Settings;

public record UserPublicInfoModel
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(255, ErrorMessage = "Name is too long")]
    public required string Name { get; init; }

    [Required(ErrorMessage = "Surname is required")]
    [MaxLength(255, ErrorMessage = "Surname is too long")]
    public required string Surname { get; init; }
}