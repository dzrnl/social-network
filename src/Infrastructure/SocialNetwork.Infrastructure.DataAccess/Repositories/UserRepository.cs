using Microsoft.EntityFrameworkCore;
using SocialNetwork.Application.Abstractions.Queries.Users;
using SocialNetwork.Application.Abstractions.Repositories;
using SocialNetwork.Application.Models;
using SocialNetwork.Infrastructure.DataAccess.Entities;
using SocialNetwork.Infrastructure.DataAccess.Mappers;

namespace SocialNetwork.Infrastructure.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SocialNetworkDbContext _context;

    public UserRepository(SocialNetworkDbContext context)
    {
        _context = context;
    }

    public async Task<User> Add(CreateUserQuery query)
    {
        var userEntity = new UserEntity
        {
            Username = query.Username,
            PasswordHash = query.PasswordHash,
            Name = query.Name
        };

        await _context.Users.AddAsync(userEntity);
        await _context.SaveChangesAsync();

        return userEntity.ToDomain();
    }

    public async Task<List<User>> FindPaged(PaginationQuery query)
    {
        return await _context.Users
            .AsNoTracking()
            .OrderBy(u => u.Username)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(u => u.ToDomain())
            .ToListAsync();
    }

    public async Task<User?> FindById(long id)
    {
        var userEntity = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        return userEntity?.ToDomain();
    }

    public async Task<bool> ChangeName(ChangeUserNameQuery query)
    {
        var affectedRows = await _context.Users
            .Where(u => u.Id == query.Id)
            .ExecuteUpdateAsync(u
                => u.SetProperty(x => x.Name, query.NewName));

        return affectedRows > 0;
    }

    public async Task<bool> Delete(long id)
    {
        var affectedRows = await _context.Users
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync();

        return affectedRows > 0;
    }
}