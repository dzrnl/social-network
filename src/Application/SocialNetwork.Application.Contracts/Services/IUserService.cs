using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Contracts.Services;

public interface IUserService
{
    Task<long> CreateUser(string name);

    Task<List<User>> GetUsers(int page, int pageSize);

    Task<User?> GetUserById(long id);
}