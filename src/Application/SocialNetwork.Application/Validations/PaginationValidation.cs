namespace SocialNetwork.Application.Validations;

public static class PaginationValidation
{
    public static string? Validate(int page, int pageSize, int maxPageSize)
    {
        if (page < 1)
        {
            return "Page number must be greater than or equal to 1";
        }

        if (pageSize < 1)
        {
            return "Page size must be greater than or equal to 1";
        }

        if (pageSize > maxPageSize)
        {
            return $"Page size must not exceed {maxPageSize}";
        }

        return null;
    }
}