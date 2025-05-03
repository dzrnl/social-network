using Microsoft.EntityFrameworkCore;
using SocialNetwork.Application.Abstractions.Queries.Messages;
using SocialNetwork.Application.Abstractions.Repositories;
using SocialNetwork.Application.Models;
using SocialNetwork.Infrastructure.DataAccess.Entities;
using SocialNetwork.Infrastructure.DataAccess.Mappers;

namespace SocialNetwork.Infrastructure.DataAccess.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly SocialNetworkDbContext _context;

    public MessageRepository(SocialNetworkDbContext context)
    {
        _context = context;
    }

    public async Task<Message> Add(CreateMessageQuery query)
    {
        var message = new MessageEntity
        {
            SenderId = query.SenderId,
            Content = query.Content
        };

        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();

        await _context.Entry(message).Reference(m => m.Sender).LoadAsync();

        return message.ToDomain();
    }

    public async Task<List<Message>> GetAll()
    {
        var messages = await _context.Messages
            .AsNoTracking()
            .Include(m => m.Sender)
            .ToListAsync();

        return messages.Select(m => m.ToDomain()).ToList();
    }
}