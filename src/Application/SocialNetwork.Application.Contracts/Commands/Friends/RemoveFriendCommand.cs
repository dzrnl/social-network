namespace SocialNetwork.Application.Contracts.Commands.Friends;

public static class RemoveFriendCommand
{
    public sealed record Request(long UserId, long FriendId);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success : Response;

        public record Failure(string Message) : Response;

        public sealed record NotFriends() : Failure("Users are not friends");
    }
}