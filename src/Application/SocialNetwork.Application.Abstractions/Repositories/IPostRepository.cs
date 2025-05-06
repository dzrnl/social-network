using SocialNetwork.Application.Abstractions.Queries;
using SocialNetwork.Application.Abstractions.Queries.Posts;
using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Abstractions.Repositories;

public interface IPostRepository
{
    Task<Post> Add(CreatePostQuery query);

    Task<List<Post>> FindPaged(PaginationQuery query);

    Task<List<Post>> FindUserPosts(long userId, PaginationQuery pagination);

    Task<bool> Delete(long id);
}