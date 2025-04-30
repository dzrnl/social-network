namespace SocialNetwork.Application.Contracts.Commands.Friends;

public static class AddFriendCommand
{
    public sealed record Request(long UserId, long FriendId);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success : Response;

        public record Failure(string Message) : Response;
        
        public record InvalidRequest(string Message) : Failure(Message);
        
        public sealed record SelfFriendship() : InvalidRequest("Friendship with self is not permitted");
        
        public sealed record AlreadyFriends() : Failure("Users are already friends");
    }
}