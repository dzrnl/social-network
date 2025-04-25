using SocialNetwork.Application.Abstractions.Queries.Users;
using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User> Add(CreateUserQuery query);

    Task<List<User>> FindAll();

    Task<User?> FindById(long id);
}