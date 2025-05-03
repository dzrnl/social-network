using Moq;
using SocialNetwork.Application.Abstractions.Auth;
using SocialNetwork.Application.Abstractions.Dtos;
using SocialNetwork.Application.Abstractions.Queries.Users;
using SocialNetwork.Application.Abstractions.Repositories;
using SocialNetwork.Application.Contracts.Commands.Auth;
using SocialNetwork.Application.Models;
using SocialNetwork.Application.Services;
using SocialNetwork.Application.Validations;
using Xunit;

namespace SocialNetwork.Tests.UnitTests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterUserTest()
    {
        var mockedUserRepository = new Mock<IUserRepository>();
        var mockedPasswordHasher = new Mock<IPasswordHasher>();
        var mockedJwtProvider = new Mock<IJwtProvider>();

        const long userId = 1;
        const string userUsername = "ivanov123";
        const string password = "password";
        const string userName = "Ivan";
        const string surname = "Ivanov";

        const string passwordHash = "passwordHash";

        mockedPasswordHasher
            .Setup(hasher => hasher.GenerateHash(password))
            .Returns(passwordHash);

        mockedUserRepository
            .Setup(repo => repo.FindByUsername(userUsername))
            .ReturnsAsync((User?)null);
        mockedUserRepository
            .Setup(repo => repo.Add(It.Is<CreateUserQuery>(q =>
                q.Username == userUsername && q.PasswordHash == passwordHash && q.Name == userName)))
            .ReturnsAsync(new User(userId, userUsername, userName, surname));

        var authService = new AuthService(mockedUserRepository.Object, mockedPasswordHasher.Object,
            mockedJwtProvider.Object);

        var response = await authService.Register(new(userUsername, password, userName, surname));

        var success = Assert.IsType<RegisterUserCommand.Response.Success>(response);

        Assert.Equal(userId, success.Id);

        mockedUserRepository.Verify(repo =>
            repo.Add(It.Is<CreateUserQuery>(q =>
                q.Username == userUsername && q.PasswordHash == passwordHash && q.Name == userName)
            ), Times.Once);
    }

    [Fact]
    public async Task RegisterUser_ShouldReturnFailure_WhenUsernameIsEmpty()
    {
        var mockedUserRepository = new Mock<IUserRepository>();
        var mockedPasswordHasher = new Mock<IPasswordHasher>();
        var mockedJwtProvider = new Mock<IJwtProvider>();

        var authService = new AuthService(mockedUserRepository.Object, mockedPasswordHasher.Object,
            mockedJwtProvider.Object);

        var response = await authService.Register(new("", "pass", "Name", "Surname"));

        var failure = Assert.IsType<RegisterUserCommand.Response.InvalidRequest>(response);

        Assert.Equal("Username cannot be empty", failure.Message);

        mockedUserRepository.Verify(repo => repo.Add(It.IsAny<CreateUserQuery>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUser_ShouldReturnFailure_WhenNameIsEmpty()
    {
        var mockedUserRepository = new Mock<IUserRepository>();
        var mockedPasswordHasher = new Mock<IPasswordHasher>();
        var mockedJwtProvider = new Mock<IJwtProvider>();

        var authService = new AuthService(mockedUserRepository.Object, mockedPasswordHasher.Object,
            mockedJwtProvider.Object);

        var response = await authService.Register(new("username", "pass", "", "Surname"));

        var failure = Assert.IsType<RegisterUserCommand.Response.InvalidRequest>(response);

        Assert.Equal("Name cannot be empty", failure.Message);

        mockedUserRepository.Verify(repo => repo.Add(It.IsAny<CreateUserQuery>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUser_ShouldReturnFailure_WhenUsernameIsTooLong()
    {
        var mockedUserRepository = new Mock<IUserRepository>();
        var mockedPasswordHasher = new Mock<IPasswordHasher>();
        var mockedJwtProvider = new Mock<IJwtProvider>();

        var longUsername = new string('a', UserValidation.MaxUsernameLength + 1);

        var authService = new AuthService(mockedUserRepository.Object, mockedPasswordHasher.Object,
            mockedJwtProvider.Object);

        var response = await authService.Register(new(longUsername, "pass", "Name", "Surname"));

        var failure = Assert.IsType<RegisterUserCommand.Response.InvalidRequest>(response);

        Assert.Equal("Username is too long", failure.Message);

        mockedUserRepository.Verify(repo => repo.Add(It.IsAny<CreateUserQuery>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUser_ShouldReturnFailure_WhenNameIsTooLong()
    {
        var mockedUserRepository = new Mock<IUserRepository>();
        var mockedPasswordHasher = new Mock<IPasswordHasher>();
        var mockedJwtProvider = new Mock<IJwtProvider>();

        var longName = new string('a', UserValidation.MaxNameLength + 1);

        var authService = new AuthService(mockedUserRepository.Object, mockedPasswordHasher.Object,
            mockedJwtProvider.Object);

        var response = await authService.Register(new("username", "pass", longName, "Surname"));

        var failure = Assert.IsType<RegisterUserCommand.Response.InvalidRequest>(response);

        Assert.Equal("Name is too long", failure.Message);

        mockedUserRepository.Verify(repo => repo.Add(It.IsAny<CreateUserQuery>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUser_ShouldReturnFailure_WhenUserAlreadyExists()
    {
        var mockedUserRepository = new Mock<IUserRepository>();
        var mockedPasswordHasher = new Mock<IPasswordHasher>();
        var mockedJwtProvider = new Mock<IJwtProvider>();

        const string username = "username";

        mockedUserRepository
            .Setup(repo => repo.ExistsByUsername(username))
            .ReturnsAsync(true);

        var authService = new AuthService(mockedUserRepository.Object, mockedPasswordHasher.Object,
            mockedJwtProvider.Object);

        var response = await authService.Register(new(username, "password", "OtherName", "OtherSurname"));

        var failure = Assert.IsType<RegisterUserCommand.Response.UserAlreadyExists>(response);

        Assert.Equal("User with this username already exists", failure.Message);

        mockedUserRepository.Verify(repo => repo.ExistsByUsername(username), Times.Once);

        mockedUserRepository.Verify(repo => repo.Add(It.IsAny<CreateUserQuery>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUser_ShouldReturnFailure_WhenRepositoryThrowsException()
    {
        var mockedUserRepository = new Mock<IUserRepository>();
        var mockedPasswordHasher = new Mock<IPasswordHasher>();
        var mockedJwtProvider = new Mock<IJwtProvider>();

        mockedUserRepository
            .Setup(repo => repo.Add(It.IsAny<CreateUserQuery>()))
            .ThrowsAsync(new Exception("Database error"));

        var authService = new AuthService(mockedUserRepository.Object, mockedPasswordHasher.Object,
            mockedJwtProvider.Object);

        var response = await authService.Register(new("username", "password", "Name", "Surname"));

        var failure = Assert.IsType<RegisterUserCommand.Response.Failure>(response);

        Assert.Equal("Unexpected error while creating user", failure.Message);
    }

    [Fact]
    public async Task LoginUserTest()
    {
        var mockedUserRepository = new Mock<IUserRepository>();
        var mockedPasswordHasher = new Mock<IPasswordHasher>();
        var mockedJwtProvider = new Mock<IJwtProvider>();

        const long userId = 1;
        const string userUsername = "ivanov123";
        const string password = "password";

        const string passwordHash = "passwordHash";
        const string token = "token";

        mockedUserRepository
            .Setup(repo => repo.FindCredentialsByUsername(userUsername))
            .ReturnsAsync(new UserCredentials(userId, userUsername, passwordHash));

        mockedPasswordHasher
            .Setup(hasher => hasher.VerifyHash(password, passwordHash))
            .Returns(true);

        mockedJwtProvider
            .Setup(provider => provider.GenerateToken(userId))
            .Returns(token);

        var authService = new AuthService(mockedUserRepository.Object, mockedPasswordHasher.Object,
            mockedJwtProvider.Object);

        var response = await authService.Login(new(userUsername, password));

        var success = Assert.IsType<LoginUserCommand.Response.Success>(response);

        Assert.Equal(token, success.Token);

        mockedUserRepository.Verify(repo =>
            repo.FindCredentialsByUsername(userUsername), Times.Once);

        mockedPasswordHasher.Verify(hasher =>
            hasher.VerifyHash(password, passwordHash), Times.Once);

        mockedJwtProvider.Verify(provider =>
            provider.GenerateToken(userId), Times.Once);
    }

    [Fact]
    public async Task LoginUserTest_ShouldReturnFailure_WhenUserNotFound()
    {
        var mockedUserRepository = new Mock<IUserRepository>();
        var mockedPasswordHasher = new Mock<IPasswordHasher>();
        var mockedJwtProvider = new Mock<IJwtProvider>();

        const string userUsername = "ivanov123";

        mockedUserRepository
            .Setup(repo => repo.FindCredentialsByUsername(userUsername))
            .ReturnsAsync((UserCredentials?)null);

        var authService = new AuthService(mockedUserRepository.Object, mockedPasswordHasher.Object,
            mockedJwtProvider.Object);

        var response = await authService.Login(new(userUsername, "pass"));

        Assert.IsType<LoginUserCommand.Response.NotFound>(response);

        mockedUserRepository.Verify(repo =>
            repo.FindCredentialsByUsername(userUsername), Times.Once);
    }

    [Fact]
    public async Task LoginUserTest_ShouldReturnFailure_WhenInvalidUsernameOrPassword()
    {
        var mockedUserRepository = new Mock<IUserRepository>();
        var mockedPasswordHasher = new Mock<IPasswordHasher>();
        var mockedJwtProvider = new Mock<IJwtProvider>();

        const long userId = 1;
        const string userUsername = "ivanov123";
        const string password = "password";

        const string passwordHash = "passwordHash";

        mockedUserRepository
            .Setup(repo => repo.FindCredentialsByUsername(userUsername))
            .ReturnsAsync(new UserCredentials(userId, userUsername, passwordHash));

        mockedPasswordHasher
            .Setup(hasher => hasher.VerifyHash(password, passwordHash))
            .Returns(false);

        var authService = new AuthService(mockedUserRepository.Object, mockedPasswordHasher.Object,
            mockedJwtProvider.Object);

        var response = await authService.Login(new(userUsername, password));

        var invalidCredentials = Assert.IsType<LoginUserCommand.Response.InvalidCredentials>(response);

        Assert.Equal("Invalid username or password", invalidCredentials.Message);

        mockedUserRepository.Verify(repo =>
            repo.FindCredentialsByUsername(userUsername), Times.Once);

        mockedPasswordHasher.Verify(provider =>
            provider.VerifyHash(password, passwordHash), Times.Once);
    }

    [Fact]
    public async Task LoginUserTest_ShouldReturnFailure_WhenRepositoryThrowsException()
    {
        var mockedUserRepository = new Mock<IUserRepository>();
        var mockedPasswordHasher = new Mock<IPasswordHasher>();
        var mockedJwtProvider = new Mock<IJwtProvider>();

        const string username = "ivanov123";

        mockedUserRepository
            .Setup(repo => repo.FindCredentialsByUsername(username))
            .ReturnsAsync((UserCredentials?)null);

        var authService = new AuthService(mockedUserRepository.Object, mockedPasswordHasher.Object,
            mockedJwtProvider.Object);

        var response = await authService.Login(new(username, "pass"));

        Assert.IsType<LoginUserCommand.Response.NotFound>(response);

        mockedUserRepository.Verify(repo => repo.FindCredentialsByUsername(username), Times.Once);
    }
}