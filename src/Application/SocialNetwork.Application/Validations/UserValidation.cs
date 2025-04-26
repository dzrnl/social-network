namespace SocialNetwork.Application.Validations;

public static class UserValidation
{
    public const int MaxUsernameLength = 50;
    public const int MaxNameLength = 255;
    
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
}