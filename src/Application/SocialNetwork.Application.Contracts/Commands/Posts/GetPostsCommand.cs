using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Contracts.Commands.Posts;

public static class GetPostsCommand
{
    public sealed record Request(int Page, int PageSize);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(List<Post> Posts) : Response;

        public record Failure(string Message) : Response;
    }
}