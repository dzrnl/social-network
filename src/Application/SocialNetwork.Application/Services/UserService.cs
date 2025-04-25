using SocialNetwork.Application.Abstractions.Queries.Users;
using SocialNetwork.Application.Abstractions.Repositories;
using SocialNetwork.Application.Contracts.Commands.Users;
using SocialNetwork.Application.Contracts.Services;

namespace SocialNetwork.Application.Services;

public class UserService : IUserService
{
    public const int MaxNameLength = 100;
    public const int MaxPageSize = 100;

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

    public async Task<GetUsersCommand.Response> GetUsers(GetUsersCommand.Request request)
    {
        if (request.Page < 1)
        {
            return new GetUsersCommand.Response.InvalidRequest("Page number must be greater than or equal to 1");
        }

        if (request.PageSize < 1)
        {
            return new GetUsersCommand.Response.InvalidRequest("Page size must be greater than or equal to 1");
        }

        if (request.PageSize > MaxPageSize)
        {
            return new GetUsersCommand.Response.InvalidRequest("Page size is too large");
        }

        var query = new PaginationQuery(request.Page, request.PageSize);

        try
        {
            var users = await _userRepository.FindPaged(query);

            return new GetUsersCommand.Response.Success(users);
        }
        catch (Exception)
        {
            return new GetUsersCommand.Response.Failure("Unexpected error while fetching users");
        }
    }

    public async Task<GetUserCommand.Response> GetUserById(GetUserCommand.Request.ById request)
    {
        try
        {
            var user = await _userRepository.FindById(request.Id);

            if (user is null)
            {
                return new GetUserCommand.Response.NotFound();
            }

            return new GetUserCommand.Response.Success(user);
        }
        catch (Exception)
        {
            return new GetUserCommand.Response.Failure("Unexpected error while fetching user");
        }
    }

    public async Task<ChangeUserNameCommand.Response> ChangeUserName(ChangeUserNameCommand.Request request)
    {
        var validationNameError = ValidateUserName(request.Name);

        if (validationNameError is not null)
        {
            return new ChangeUserNameCommand.Response.InvalidRequest(validationNameError);
        }

        var query = new ChangeUserNameQuery(request.Id, request.Name);

        try
        {
            var changed = await _userRepository.ChangeUserName(query);

            if (changed == false)
            {
                return new ChangeUserNameCommand.Response.NotFound();
            }

            return new ChangeUserNameCommand.Response.Success();
        }
        catch (Exception)
        {
            return new ChangeUserNameCommand.Response.Failure("Unexpected error while changing user name");
        }
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