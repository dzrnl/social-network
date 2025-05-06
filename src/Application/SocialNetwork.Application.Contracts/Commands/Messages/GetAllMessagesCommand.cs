using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Contracts.Commands.Messages;

public static class GetAllMessagesCommand
{
    public sealed record Request;

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(List<Message> Messages) : Response;

        public record Failure(string Message) : Response;
    }
}