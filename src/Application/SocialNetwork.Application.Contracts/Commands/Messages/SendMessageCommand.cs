using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Contracts.Commands.Messages;

public static class SendMessageCommand
{
    public sealed record Request(long SenderId, string Content);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(Message Message) : Response;

        public record Failure(string Message) : Response;
    }
}