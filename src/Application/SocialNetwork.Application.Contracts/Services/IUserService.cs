using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Contracts.Services;

public interface IUserService
{
    Task<long> CreateUser(string name);

    Task<List<User>> GetAllUsers();

    Task<User?> GetUserById(long id);
}