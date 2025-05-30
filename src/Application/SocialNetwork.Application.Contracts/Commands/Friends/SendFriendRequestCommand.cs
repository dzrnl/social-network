using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Contracts.Commands.Friends;

public static class SendFriendRequestCommand
{
    public sealed record Request(long UserId, long FriendId);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(FriendRequest FriendRequest) : Response;

        public record Failure(string Message) : Response;

        public sealed record SelfFriendship() : Failure("Friendship with self is not permitted");

        public sealed record UserNotFound() : Failure("User not found");

        public sealed record AlreadyFriends() : Failure("Users are already friends");

        public sealed record RequestAlreadySent() : Failure("Request is already sent");
    }
}