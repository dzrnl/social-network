namespace SocialNetwork.Application.Contracts.Commands.Users;

public static class CreateUserCommand
{
    public sealed record Request(string Name);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(long Id) : Response;

        public record Failure(string Message) : Response;
        
        public sealed record InvalidRequest(string Message) : Failure(Message);
    }
}