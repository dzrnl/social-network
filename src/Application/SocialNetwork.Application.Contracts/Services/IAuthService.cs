using SocialNetwork.Application.Contracts.Commands.Auth;

namespace SocialNetwork.Application.Contracts.Services;

public interface IAuthService
{
    Task<RegisterUserCommand.Response> Register(RegisterUserCommand.Request request);

    Task<LoginUserCommand.Response> Login(LoginUserCommand.Request request);
}