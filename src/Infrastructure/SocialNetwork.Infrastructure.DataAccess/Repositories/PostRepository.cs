using Microsoft.EntityFrameworkCore;
using SocialNetwork.Application.Abstractions.Queries;
using SocialNetwork.Application.Abstractions.Queries.Posts;
using SocialNetwork.Application.Abstractions.Repositories;
using SocialNetwork.Application.Models;
using SocialNetwork.Infrastructure.DataAccess.Entities;
using SocialNetwork.Infrastructure.DataAccess.Mappers;

namespace SocialNetwork.Infrastructure.DataAccess.Repositories;

public class PostRepository : IPostRepository
{
    private readonly SocialNetworkDbContext _context;

    public PostRepository(SocialNetworkDbContext context)
    {
        _context = context;
    }

    public async Task<Post> Add(CreatePostQuery query)
    {
        var post = new PostEntity
        {
            AuthorId = query.AuthorId,
            Content = query.Content
        };

        await _context.Posts.AddAsync(post);
        await _context.SaveChangesAsync();

        await _context.Entry(post).Reference(p => p.Author).LoadAsync();

        return post.ToDomain();
    }

    public async Task<List<Post>> FindPaged(PaginationQuery query)
    {
        var posts = await _context.Posts
            .AsNoTracking()
            .Include(p => p.Author)
            .OrderByDescending(p => p.PublishedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return posts.Select(p => p.ToDomain()).ToList();
    }

    public async Task<List<Post>> FindUserPosts(long userId, PaginationQuery query)
    {
        var posts = await _context.Posts
            .AsNoTracking()
            .Where(p => p.AuthorId == userId)
            .Include(p => p.Author)
            .OrderByDescending(p => p.PublishedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return posts.Select(p => p.ToDomain()).ToList();
    }

    public async Task<bool> Delete(long id)
    {
        var affectedRows = await _context.Posts
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync();

        return affectedRows > 0;
    }
}