using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Contracts.Commands.Users;

public static class GetUsersCommand
{
    public sealed record Request(int Page, int PageSize);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(List<UserPreview> Users) : Response;

        public record Failure(string Message) : Response;
    }
}