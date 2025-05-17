using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Contracts.Commands.Friends;

public abstract class GetFriendStatusCommand
{
    public sealed record Request(long UsedId, long FriendId);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(FriendStatus FriendStatus) : Response;

        public record Failure(string Message) : Response;

        public sealed record UserNotFound() : Failure("User not found");
    }
}