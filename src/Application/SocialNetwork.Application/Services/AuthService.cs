using SocialNetwork.Application.Abstractions.Auth;
using SocialNetwork.Application.Abstractions.Queries.Users;
using SocialNetwork.Application.Abstractions.Repositories;
using SocialNetwork.Application.Contracts.Commands.Auth;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Validations;

namespace SocialNetwork.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<RegisterUserCommand.Response> Register(RegisterUserCommand.Request request)
    {
        if (UserValidation.ValidateUsername(request.Username) is { } validationUsernameError)
        {
            return new RegisterUserCommand.Response.InvalidRequest(validationUsernameError);
        }

        if (UserValidation.ValidateName(request.Name) is { } validationNameError)
        {
            return new RegisterUserCommand.Response.InvalidRequest(validationNameError);
        }

        if (UserValidation.ValidateSurname(request.Surname) is { } validationSurnameError)
        {
            return new RegisterUserCommand.Response.InvalidRequest(validationSurnameError);
        }

        if (UserValidation.ValidatePassword(request.Password) is { } validationPasswordError)
        {
            return new RegisterUserCommand.Response.InvalidRequest(validationPasswordError);
        }

        var hashedPassword = _passwordHasher.GenerateHash(request.Password);

        var query = new CreateUserQuery(request.Username, hashedPassword, request.Name, request.Surname);

        try
        {
            var userExists = await _userRepository.ExistsByUsername(query.Username);

            if (userExists)
            {
                return new RegisterUserCommand.Response.UserAlreadyExists();
            }

            var user = await _userRepository.Add(query);

            return new RegisterUserCommand.Response.Success(user.Id);
        }
        catch (Exception)
        {
            return new RegisterUserCommand.Response.Failure("Unexpected error while creating user");
        }
    }

    public async Task<LoginUserCommand.Response> Login(LoginUserCommand.Request request)
    {
        try
        {
            var credentials = await _userRepository.FindCredentialsByUsername(request.Username);

            if (credentials is null)
            {
                return new LoginUserCommand.Response.NotFound();
            }

            var result = _passwordHasher.VerifyHash(request.Password, credentials.PasswordHash);

            if (!result)
            {
                return new LoginUserCommand.Response.InvalidCredentials();
            }

            var token = _jwtProvider.GenerateToken(credentials.Id);

            return new LoginUserCommand.Response.Success(token);
        }
        catch (Exception)
        {
            return new LoginUserCommand.Response.Failure("Unexpected error while logging in user");
        }
    }
}