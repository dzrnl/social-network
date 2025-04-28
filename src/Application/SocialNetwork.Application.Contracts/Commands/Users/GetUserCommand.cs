using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Contracts.Commands.Users;

public static class GetUserCommand
{
    public abstract record Request
    {
        private Request() { }

        public sealed record ById(long Id) : Request;
        
        public sealed record ByUsername(string Username) : Request;
    }

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(User User) : Response;

        public record Failure(string Message) : Response;
        
        public sealed record NotFound() : Failure("User not found");
    }
}