namespace SocialNetwork.Application.Contracts.Commands.Friends;

public static class DeclineFriendRequestCommand
{
    public sealed record Request(long RequestId);

    public abstract record Response
    {
        private Response() { }

        public sealed record Success : Response;

        public record Failure(string Message) : Response;
        
        public sealed record NoPendingRequest() : Failure("No pending request");
    }
}