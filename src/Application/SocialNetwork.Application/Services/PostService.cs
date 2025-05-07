using SocialNetwork.Application.Abstractions.Queries;
using SocialNetwork.Application.Abstractions.Queries.Posts;
using SocialNetwork.Application.Abstractions.Repositories;
using SocialNetwork.Application.Contracts.Commands.Posts;
using SocialNetwork.Application.Contracts.Services;

namespace SocialNetwork.Application.Services;

public class PostService : IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;

    public PostService(IPostRepository postRepository, IUserRepository userRepository)
    {
        _postRepository = postRepository;
        _userRepository = userRepository;
    }

    public async Task<CreatePostCommand.Response> CreatePost(CreatePostCommand.Request request)
    {
        var query = new CreatePostQuery(request.AuthorId, request.Content);

        try
        {
            var userExists = await _userRepository.ExistsById(query.AuthorId);

            if (!userExists)
            {
                return new CreatePostCommand.Response.UserNotFound();
            }

            var post = await _postRepository.Add(query);

            return new CreatePostCommand.Response.Success(post);
        }
        catch (Exception)
        {
            return new CreatePostCommand.Response.Failure("Unexpected error while creating post");
        }
    }

    public async Task<GetPostsCommand.Response> GetPosts(GetPostsCommand.Request request)
    {
        var paginationQuery = new PaginationQuery(request.Page, request.PageSize);

        try
        {
            var posts = await _postRepository.FindPaged(paginationQuery);

            return new GetPostsCommand.Response.Success(posts);
        }
        catch (Exception)
        {
            return new GetPostsCommand.Response.Failure("Unexpected error while fetching posts");
        }
    }

    public async Task<GetUserPostsCommand.Response> GetUserPosts(GetUserPostsCommand.Request request)
    {
        var paginationQuery = new PaginationQuery(request.Page, request.PageSize);

        try
        {
            var userExists = await _userRepository.ExistsById(request.UsedId);

            if (!userExists)
            {
                return new GetUserPostsCommand.Response.UserNotFound();
            }

            var posts = await _postRepository.FindUserPosts(request.UsedId, paginationQuery);

            return new GetUserPostsCommand.Response.Success(posts);
        }
        catch (Exception)
        {
            return new GetUserPostsCommand.Response.Failure("Unexpected error while fetching user posts");
        }
    }

    public async Task<DeletePostCommand.Response> DeletePost(DeletePostCommand.Request request)
    {
        try
        {
            var deleted = await _postRepository.Delete(request.Id);

            if (deleted == false)
            {
                return new DeletePostCommand.Response.NotFound();
            }

            return new DeletePostCommand.Response.Success();
        }
        catch (Exception)
        {
            return new DeletePostCommand.Response.Failure("Unexpected error while deleting post");
        }
    }
}