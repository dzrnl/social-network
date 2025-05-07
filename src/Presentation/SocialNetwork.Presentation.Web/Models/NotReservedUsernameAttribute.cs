using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Presentation.Web.Models;

public class NotReservedUsernameAttribute : ValidationAttribute
{
    private static readonly HashSet<string> ReservedUsernames = new(StringComparer.OrdinalIgnoreCase)
    {
        "login",
        "logout",
        "register",
        "search",
        "feed",
        "messages",
        "friends",
        "settings"
    };

    public NotReservedUsernameAttribute()
    {
        ErrorMessage = "This username is reserved and cannot be used";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string username && ReservedUsernames.Contains(username))
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}