using SocialNetwork.Application.Abstractions.Dtos;
using SocialNetwork.Application.Abstractions.Queries.Users;
using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User> Add(CreateUserQuery query);

    Task<List<User>> FindPaged(PaginationQuery query);

    Task<User?> FindById(long id);
    
    Task<User?> FindByUsername(string username);

    Task<bool> ChangeName(ChangeUserNameQuery query);

    Task<bool> Delete(long id);
    
    Task<UserCredentials?> FindCredentialsByUsername(string username);
}