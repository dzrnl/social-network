namespace SocialNetwork.Application.Validations;

public static class PostValidation
{
    public const int MaxLength = 2000;

    public static string? ValidateContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return "Post content cannot be empty or whitespace";
        }

        if (content.Length > MaxLength)
        {
            return $"Post content length must not exceed {MaxLength} characters";
        }

        return null;
    }
}