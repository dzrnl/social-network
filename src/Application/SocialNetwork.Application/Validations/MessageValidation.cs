namespace SocialNetwork.Application.Validations;

public class MessageValidation
{
    public const int MaxLength = 2000;
    
    public static string? ValidateContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return "Message cannot be empty or whitespace";
        }

        if (content.Length > MaxLength)
        {
            return $"Message length must not exceed {MaxLength} characters";
        }

        return null;
    }
}