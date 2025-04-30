using Microsoft.EntityFrameworkCore;
using SocialNetwork.Application.Abstractions.Repositories;
using SocialNetwork.Application.Models;
using SocialNetwork.Infrastructure.DataAccess.Mappers;

namespace SocialNetwork.Infrastructure.DataAccess.Repositories;

public class FriendshipRepository : IFriendshipRepository
{
    private readonly SocialNetworkDbContext _context;

    public FriendshipRepository(SocialNetworkDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddFriend(long userId, long friendId)
    {
        var user = await _context.Users
            .Include(u => u.Friends)
            .SingleAsync(u => u.Id == userId);

        if (user.Friends.Any(f => f.Id == friendId))
        {
            return false;
        }

        var friend = await _context.Users
            .Include(u => u.Friends)
            .SingleAsync(u => u.Id == friendId);

        user.Friends.Add(friend);
        friend.Friends.Add(user);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveFriend(long userId, long friendId)
    {
        var user = await _context.Users
            .Include(u => u.Friends)
            .SingleAsync(u => u.Id == userId);

        if (user.Friends.All(f => f.Id != friendId))
        {
            return false;
        }

        var friend = await _context.Users
            .Include(u => u.Friends)
            .SingleAsync(u => u.Id == friendId);

        user.Friends.Remove(friend);
        friend.Friends.Remove(user);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AreFriends(long userId1, long userId2)
    {
        var user = await _context.Users
            .Include(u => u.Friends)
            .FirstOrDefaultAsync(u => u.Id == userId1);

        if (user == null)
        {
            return false;
        }

        return user.Friends.Any(f => f.Id == userId2);
    }

    public async Task<List<User>> FindFriends(long userId)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.Friends)
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Friends)
            .Select(u => u.ToDomain())
            .ToListAsync();
    }
}