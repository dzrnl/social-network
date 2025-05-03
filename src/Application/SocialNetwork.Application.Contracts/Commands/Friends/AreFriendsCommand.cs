namespace SocialNetwork.Application.Contracts.Commands.Friends;

public static class AreFriendsCommand
{
    public sealed record Request(long UserId, long FriendId);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(bool AreFriends) : Response;

        public record Failure(string Message) : Response;
        
        public sealed record UserNotFound() : Failure("User not found");
    }
}