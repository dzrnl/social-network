using SocialNetwork.Application.Contracts.Commands.Posts;

namespace SocialNetwork.Application.Contracts.Services;

public interface IPostService
{
    Task<CreatePostCommand.Response> CreatePost(CreatePostCommand.Request request);
    
    Task<GetPostsCommand.Response> GetPosts(GetPostsCommand.Request request);
    
    Task<GetUserPostsCommand.Response> GetUserPosts(GetUserPostsCommand.Request request);
    
    Task<DeletePostCommand.Response> DeletePost(DeletePostCommand.Request request);
}