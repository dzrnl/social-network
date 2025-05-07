using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Presentation.Web.Models.Posts;

public record CreatePostModel
{
    [Required(ErrorMessage = "Post content cannot be empty")]
    [MaxLength(2000, ErrorMessage = "Post content cannot be longer than 2000 characters")]
    public required string Content { get; init; }
}