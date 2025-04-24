using SocialNetwork.Application.Abstractions.Queries.Users;
using SocialNetwork.Application.Abstractions.Repositories;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<long> CreateUser(string name)
    {
        var query = new CreateUserQuery(name);

        var user = await _userRepository.Add(query);

        return user.Id;
    }

    public async Task<List<User>> GetAllUsers()
    {
        return await _userRepository.FindAll();
    }

    public async Task<User?> GetUserById(long id)
    {
        return await _userRepository.FindById(id);
    }
}