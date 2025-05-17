using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Contracts.Commands.Friends;

public static class GetUserSentRequestsCommand
{
    public sealed record Request(long UsedId);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(List<FriendRequest> SentRequests) : Response;

        public record Failure(string Message) : Response;

        public sealed record UserNotFound() : Failure("User not found");
    }
}