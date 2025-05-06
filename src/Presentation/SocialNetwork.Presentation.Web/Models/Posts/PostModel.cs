using SocialNetwork.Application.Models;
using SocialNetwork.Presentation.Web.Models.Users;

namespace SocialNetwork.Presentation.Web.Models.Posts;

public record PostModel(long Id, UserPreviewModel? Author, string Content, DateTime PublishedAt)
{
    public static PostModel ToViewModel(Post post)
    {
        var author = post.Author != null ? UserPreviewModel.ToViewModel(post.Author) : null;

        return new PostModel(
            post.Id,
            author,
            post.Content,
            post.PublishedAt);
    }
}