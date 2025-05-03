using Microsoft.EntityFrameworkCore;
using SocialNetwork.Application.Abstractions.Dtos;
using SocialNetwork.Application.Abstractions.Queries;
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
            Name = query.Name,
            Surname = query.Surname
        };

        await _context.Users.AddAsync(userEntity);
        await _context.SaveChangesAsync();

        return userEntity.ToDomain();
    }

    public async Task<List<User>> FindPaged(PaginationQuery query)
    {
        var users = await _context.Users
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return users.Select(u => u.ToDomain()).ToList();
    }

    public async Task<List<User>> SearchPaged(string query, PaginationQuery pagination)
    {
        var usersQuery = _context.Users
            .AsNoTracking()
            .Where(u =>
                EF.Functions.ILike(u.Username, query + "%") ||
                EF.Functions.ILike(u.Name, query + "%") ||
                EF.Functions.ILike(u.Surname, query + "%"));

        var users = await usersQuery
            .OrderBy(u => u.Id)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return users.Select(u => u.ToDomain()).ToList();
    }

    public async Task<User?> FindById(long id)
    {
        var userEntity = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        return userEntity?.ToDomain();
    }

    public async Task<User?> FindByUsername(string username)
    {
        var userEntity = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username);

        return userEntity?.ToDomain();
    }

    public async Task<bool> ExistsById(long id)
    {
        return await _context.Users
            .AnyAsync(u => u.Id == id);
    }

    public async Task<bool> ExistsByUsername(string username)
    {
        return await _context.Users
            .AnyAsync(u => u.Username == username);
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

    public async Task<UserCredentials?> FindCredentialsByUsername(string username)
    {
        var userEntity = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username);

        if (userEntity is null)
        {
            return null;
        }

        return new UserCredentials(userEntity.Id, userEntity.Username, userEntity.PasswordHash);
    }
}