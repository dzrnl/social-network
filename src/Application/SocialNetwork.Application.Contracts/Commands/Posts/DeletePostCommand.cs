namespace SocialNetwork.Application.Contracts.Commands.Posts;

public static class DeletePostCommand
{
    public sealed record Request(long Id);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success : Response;

        public record Failure(string Message) : Response;

        public sealed record NotFound() : Failure("Post not found");
    }
}