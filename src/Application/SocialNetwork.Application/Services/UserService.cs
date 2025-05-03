using SocialNetwork.Application.Abstractions.Queries;
using SocialNetwork.Application.Abstractions.Queries.Users;
using SocialNetwork.Application.Abstractions.Repositories;
using SocialNetwork.Application.Contracts.Commands.Users;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Models;
using SocialNetwork.Application.Validations;

namespace SocialNetwork.Application.Services;

public class UserService : IUserService
{
    public const int MaxPageSize = 100;

    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<GetUsersCommand.Response> GetUsers(GetUsersCommand.Request request)
    {
        if (PaginationValidation.Validate(request.Page, request.PageSize, MaxPageSize) is { } paginationError)
        {
            return new GetUsersCommand.Response.InvalidRequest(paginationError);
        }

        var paginationQuery = new PaginationQuery(request.Page, request.PageSize);

        try
        {
            List<User> users;

            if (!string.IsNullOrWhiteSpace(request.Query))
            {
                users = await _userRepository.SearchPaged(request.Query, paginationQuery);
            }
            else
            {
                users = await _userRepository.FindPaged(paginationQuery);
            }

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

    public async Task<GetUserCommand.Response> GetUserByUsername(GetUserCommand.Request.ByUsername request)
    {
        try
        {
            var user = await _userRepository.FindByUsername(request.Username);

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
        if (UserValidation.ValidateName(request.NewName) is { } validationNameError)
        {
            return new ChangeUserNameCommand.Response.InvalidRequest(validationNameError);
        }

        var query = new ChangeUserNameQuery(request.Id, request.NewName);

        try
        {
            var changed = await _userRepository.ChangeName(query);

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

    public async Task<DeleteUserCommand.Response> DeleteUser(DeleteUserCommand.Request request)
    {
        try
        {
            var deleted = await _userRepository.Delete(request.Id);

            if (deleted == false)
            {
                return new DeleteUserCommand.Response.NotFound();
            }

            return new DeleteUserCommand.Response.Success();
        }
        catch (Exception)
        {
            return new DeleteUserCommand.Response.Failure("Unexpected error while deleting user");
        }
    }
}