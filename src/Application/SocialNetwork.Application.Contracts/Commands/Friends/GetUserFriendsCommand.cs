using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Contracts.Commands.Friends;

public static class GetUserFriendsCommand
{
    public sealed record Request(long UsedId, int Page, int PageSize);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(List<UserPreview> Friends) : Response;

        public record Failure(string Message) : Response;

        public sealed record UserNotFound() : Failure("User not found");
    }
}