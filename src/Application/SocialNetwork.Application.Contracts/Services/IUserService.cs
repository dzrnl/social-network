using SocialNetwork.Application.Contracts.Commands.Users;

namespace SocialNetwork.Application.Contracts.Services;

public interface IUserService
{
    Task<GetUsersCommand.Response> GetUsers(GetUsersCommand.Request request);

    Task<GetUserCommand.Response> GetUserById(GetUserCommand.Request.ById request);

    Task<ChangeUserNameCommand.Response> ChangeUserName(ChangeUserNameCommand.Request request);

    Task<DeleteUserCommand.Response> DeleteUser(DeleteUserCommand.Request request);
}