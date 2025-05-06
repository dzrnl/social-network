using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Contracts.Commands.Posts;

public static class CreatePostCommand
{
    public sealed record Request(long AuthorId, string Content);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(Post Post) : Response;

        public record Failure(string Message) : Response;

        public sealed record InvalidRequest(string Message) : Failure(Message);

        public sealed record UserNotFound() : Failure("User not found");
    }
}