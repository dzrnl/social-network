namespace SocialNetwork.Application.Contracts.Commands.Auth;

public static class RegisterUserCommand
{
    public sealed record Request(string Username, string Password, string Name, string Surname);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(long Id) : Response;

        public record Failure(string Message) : Response;

        public sealed record InvalidRequest(string Message) : Failure(Message);

        public sealed record UserAlreadyExists() : Failure("User with this username already exists");
    }
}