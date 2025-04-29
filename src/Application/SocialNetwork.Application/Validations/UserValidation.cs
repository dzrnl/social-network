namespace SocialNetwork.Application.Validations;

public static class UserValidation
{
    public const int MaxUsernameLength = 50;
    public const int MaxNameLength = 255;
    public const int MinPasswordLength = 6;
    public const int MaxPasswordLength = 64;
    
    private static readonly List<string> ReservedUsernames =
    [
        "login",
        "logout",
        "register",
        "search",
        "feed",
        "messages",
        "friends",
        "settings"
    ];
    
    public static string? ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return "Username cannot be empty";
        }

        if (username.Length > MaxUsernameLength)
        {
            return "Username is too long";
        }

        if (ReservedUsernames.Contains(username))
        {
            return "This username is reserved and cannot be used";
        }

        return null;
    }

    public static string? ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "Name cannot be empty";
        }

        if (name.Length > MaxNameLength)
        {
            return "Name is too long";
        }

        return null;
    }

    public static string? ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return "Password cannot be empty";
        }
        
        if (password.Length < MinPasswordLength)
        {
            return "Password is too short";
        }

        if (password.Length > MaxPasswordLength)
        {
            return "Password is too long";
        }

        return null;
    }
}