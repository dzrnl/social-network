using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Presentation.Web.Models;

public record PaginatedRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
    public int Page { get; init; } = 1;

    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
    public int PageSize { get; init; } = 10;
}