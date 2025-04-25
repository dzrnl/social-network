using SocialNetwork.Application.Abstractions.Queries.Users;
using SocialNetwork.Application.Abstractions.Repositories;
using SocialNetwork.Application.Contracts.Commands.Users;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Models;

namespace SocialNetwork.Application.Services;

public class UserService : IUserService
{
    public const int MaxNameLength = 100;

    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<CreateUserCommand.Response> CreateUser(CreateUserCommand.Request request)
    {
        var validationNameError = ValidateUserName(request.Name);

        if (validationNameError is not null)
        {
            return new CreateUserCommand.Response.InvalidRequest(validationNameError);
        }

        var query = new CreateUserQuery(request.Name);

        try
        {
            var user = await _userRepository.Add(query);

            return new CreateUserCommand.Response.Success(user.Id);
        }
        catch (Exception)
        {
            return new CreateUserCommand.Response.Failure("Unexpected error while creating user");
        }
    }

    public async Task<List<User>> GetUsers(int page, int pageSize)
    {
        var query = new PaginationQuery(page, pageSize);

        return await _userRepository.FindPaged(query);
    }

    public async Task<User?> GetUserById(long id)
    {
        return await _userRepository.FindById(id);
    }

    public async Task ChangeUserName(long id, string name)
    {
        var query = new ChangeUserNameQuery(id, name);

        await _userRepository.ChangeUserName(query);
    }

    private static string? ValidateUserName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "Name cannot be empty";
        }

        if (name.Length > MaxNameLength)
        {
            return "Name is too long";
        }

        return null;
    }
}