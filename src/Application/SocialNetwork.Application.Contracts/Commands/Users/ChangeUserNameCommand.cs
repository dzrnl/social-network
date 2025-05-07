namespace SocialNetwork.Application.Contracts.Commands.Users;

public static class ChangeUserNameCommand
{
    public sealed record Request(long Id, string NewName);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success : Response;

        public record Failure(string Message) : Response;

        public sealed record NotFound() : Failure("User not found");
    }
}