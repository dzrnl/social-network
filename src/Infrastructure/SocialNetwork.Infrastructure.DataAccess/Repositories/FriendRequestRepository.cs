using Microsoft.EntityFrameworkCore;
using SocialNetwork.Application.Abstractions.Repositories;
using SocialNetwork.Application.Models;
using SocialNetwork.Infrastructure.DataAccess.Entities;
using SocialNetwork.Infrastructure.DataAccess.Mappers;

namespace SocialNetwork.Infrastructure.DataAccess.Repositories;

public class FriendRequestRepository : IFriendRequestRepository
{
    private readonly SocialNetworkDbContext _context;

    public FriendRequestRepository(SocialNetworkDbContext context)
    {
        _context = context;
    }

    public async Task<FriendRequest> Add(long fromUserId, long toUserId)
    {
        var fromUser = await _context.Users
            .Include(u => u.SentRequests)
            .SingleAsync(u => u.Id == fromUserId);

        var toUser = await _context.Users
            .Include(u => u.ReceivedRequests)
            .SingleAsync(u => u.Id == toUserId);

        var request = new FriendRequestEntity
        {
            FromUser = fromUser,
            ToUser = toUser
        };

        await _context.FriendRequests.AddAsync(request);
        await _context.SaveChangesAsync();

        return request.ToDomain();
    }

    public async Task<FriendRequest?> FindById(long id)
    {
        var friendRequestEntity = await _context.FriendRequests
            .AsNoTracking()
            .Include(fr => fr.FromUser)
            .Include(fr => fr.ToUser)
            .FirstOrDefaultAsync(fr => fr.Id == id);

        return friendRequestEntity?.ToDomain();
    }

    public async Task<FriendRequest?> FindByUserIds(long userId1, long userId2)
    {
        var friendRequestEntity = await _context.FriendRequests
            .AsNoTracking()
            .Include(fr => fr.FromUser)
            .Include(fr => fr.ToUser)
            .FirstOrDefaultAsync(fr => fr.FromUser.Id == userId1 && fr.ToUser.Id == userId2
                                       || fr.FromUser.Id == userId2 && fr.ToUser.Id == userId1);

        return friendRequestEntity?.ToDomain();
    }

    public async Task<bool> ExistsByUsers(long fromUserId, long toUserId)
    {
        return await _context.FriendRequests
            .AnyAsync(fr => fr.FromUserId == fromUserId && fr.ToUserId == toUserId);
    }

    public async Task<bool> Delete(long id)
    {
        var request = await _context.FriendRequests
            .FirstOrDefaultAsync(fr => fr.Id == id);

        if (request == null)
        {
            return false;
        }

        _context.FriendRequests.Remove(request);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<FriendRequest>> GetIncomingRequests(long userId)
    {
        var requests = await _context.FriendRequests
            .Where(fr => fr.ToUserId == userId)
            .Include(fr => fr.FromUser)
            .Include(fr => fr.ToUser)
            .ToListAsync();

        return requests.Select(u => u.ToDomain()).ToList();
    }

    public async Task<List<FriendRequest>> GetSentRequests(long userId)
    {
        var requests = await _context.FriendRequests
            .Where(fr => fr.FromUserId == userId)
            .Include(fr => fr.FromUser)
            .Include(fr => fr.ToUser)
            .ToListAsync();

        return requests.Select(u => u.ToDomain()).ToList();
    }
}