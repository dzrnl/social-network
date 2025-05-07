using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Presentation.Web.Models.Messages;

public record SendMessageModel
{
    [Required(ErrorMessage = "Message content cannot be empty")]
    [MaxLength(2000, ErrorMessage = "Message content cannot be longer than 2000 characters")]
    public required string Content { get; init; }
}