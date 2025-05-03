using SocialNetwork.Application.Models;
using SocialNetwork.Presentation.Web.Models.Users;

namespace SocialNetwork.Presentation.Web.Models.Messages;

public record MessageModel(long Id, UserPreviewModel? Sender, string Content, DateTime SentAt)
{
    public static MessageModel ToViewModel(Message message)
    {
        var sender = message.Sender != null ? UserPreviewModel.ToViewModel(message.Sender) : null;

        return new MessageModel(
            message.Id,
            sender,
            message.Content,
            message.SentAt);
    }
}