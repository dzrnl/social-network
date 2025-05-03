namespace SocialNetwork.Application.Contracts.Commands.Auth;

public class LoginUserCommand
{
    public sealed record Request(string Username, string Password);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(string Token) : Response;

        public record Failure(string Message) : Response;

        public sealed record NotFound() : Failure("User not found");

        public sealed record InvalidCredentials() : Failure("Invalid username or password");
    }
}