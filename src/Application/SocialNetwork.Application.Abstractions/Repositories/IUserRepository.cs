using SocialNetwork.Application.Abstractions.Dtos;
using SocialNetwork.Application.Abstractions.Queries;
using SocialNetwork.Application.Abstractions.Queries.Users;
using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User> Add(CreateUserQuery query);

    Task<List<UserPreview>> FindPaged(PaginationQuery query);

    Task<List<UserPreview>> SearchPaged(string query, PaginationQuery pagination);

    Task<User?> FindById(long id);

    Task<User?> FindByUsername(string username);

    Task<bool> ExistsById(long id);

    Task<bool> ExistsByUsername(string username);

    Task<bool> ChangeName(ChangeUserNameQuery query);

    Task<bool> ChangeSurname(ChangeUserSurnameQuery query);

    Task<bool> Delete(long id);

    Task<UserCredentials?> FindCredentialsByUsername(string username);
}