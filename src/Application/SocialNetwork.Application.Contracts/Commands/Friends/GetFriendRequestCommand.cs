using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Contracts.Commands.Friends;

public abstract class GetFriendRequestCommand
{
    public abstract record Request
    {
        private Request() { }

        public sealed record ById(long Id) : Request;

        public sealed record ByUsers(long UserId1, long UserId2) : Request;
    }

    public abstract record Response
    {
        private Response() { }

        public sealed record Success(FriendRequest FriendRequest) : Response;

        public record Failure(string Message) : Response;
        
        public sealed record UserNotFound() : Failure("User not found");

        public sealed record NotFound() : Failure("Friend request not found");
    }
}