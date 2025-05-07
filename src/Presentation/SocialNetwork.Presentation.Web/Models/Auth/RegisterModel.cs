using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Presentation.Web.Models.Auth;

public record RegisterModel
{
    [Required(ErrorMessage = "Username is required")]
    [MaxLength(50, ErrorMessage = "Username is too long")]
    [RegularExpression("^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores")]
    [NotReservedUsername]
    public required string Username { get; init; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    [MaxLength(64, ErrorMessage = "Password must be at most 64 characters")]
    public required string Password { get; init; }
    
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(255, ErrorMessage = "Name is too long")]
    public required string Name { get; init; }
    
    [Required(ErrorMessage = "Surname is required")]
    [MaxLength(255, ErrorMessage = "Surname is too long")]
    public required string Surname { get; init; }
}