using SocialNetwork.Application.Contracts.Commands.Users;
using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Contracts.Services;

public interface IUserService
{
    Task<CreateUserCommand.Response> CreateUser(CreateUserCommand.Request request);

    Task<List<User>> GetUsers(int page, int pageSize);

    Task<User?> GetUserById(long id);

    Task ChangeUserName(long id, string name);
}