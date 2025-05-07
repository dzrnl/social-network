using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Contracts.Commands.Posts;

public static class GetUserPostsCommand
{
    public sealed record Request(long UsedId, int Page, int PageSize);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(List<Post> Posts) : Response;

        public record Failure(string Message) : Response;

        public sealed record UserNotFound() : Failure("User not found");
    }
}