using SocialNetwork.Application.Abstractions.Queries.Users;
using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User> Add(CreateUserQuery query);

    Task<List<User>> FindPaged(PaginationQuery query);

    Task<User?> FindById(long id);

    Task<bool> ChangeUserName(ChangeUserNameQuery query);
}