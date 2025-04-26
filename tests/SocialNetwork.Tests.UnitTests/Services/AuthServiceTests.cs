using Moq;
using SocialNetwork.Application.Abstractions.Auth;
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
            .ReturnsAsync(new User(userId, userUsername, userName));

        var authService = new AuthService(mockedUserRepository.Object, mockedPasswordHasher.Object,
            mockedJwtProvider.Object);

        var response = await authService.Register(new(userUsername, password, userName));

        var success = Assert.IsType<RegisterUserCommand.Response.Success>(response);

        Assert.Equal(userId, success.Id);

        mockedUserRepository.Verify(repo =>
            repo.Add(It.Is<CreateUserQuery>(q =>
                q.Username == userUsername && q.PasswordHash == passwordHash && q.Name == userName)
            ), Times.Once);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnFailure_WhenUsernameIsEmpty()
    {
        var mockedUserRepository = new Mock<IUserRepository>();
        var mockedPasswordHasher = new Mock<IPasswordHasher>();
        var mockedJwtProvider = new Mock<IJwtProvider>();

        var authService = new AuthService(mockedUserRepository.Object, mockedPasswordHasher.Object,
            mockedJwtProvider.Object);

        var response = await authService.Register(new("", "pass", "Name"));

        var failure = Assert.IsType<RegisterUserCommand.Response.InvalidRequest>(response);

        Assert.Equal("Username cannot be empty", failure.Message);

        mockedUserRepository.Verify(repo => repo.Add(It.IsAny<CreateUserQuery>()), Times.Never);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnFailure_WhenNameIsEmpty()
    {
        var mockedUserRepository = new Mock<IUserRepository>();
        var mockedPasswordHasher = new Mock<IPasswordHasher>();
        var mockedJwtProvider = new Mock<IJwtProvider>();

        var authService = new AuthService(mockedUserRepository.Object, mockedPasswordHasher.Object,
            mockedJwtProvider.Object);

        var response = await authService.Register(new("username", "pass", ""));

        var failure = Assert.IsType<RegisterUserCommand.Response.InvalidRequest>(response);

        Assert.Equal("Name cannot be empty", failure.Message);

        mockedUserRepository.Verify(repo => repo.Add(It.IsAny<CreateUserQuery>()), Times.Never);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnFailure_WhenUsernameIsTooLong()
    {
        var mockedUserRepository = new Mock<IUserRepository>();
        var mockedPasswordHasher = new Mock<IPasswordHasher>();
        var mockedJwtProvider = new Mock<IJwtProvider>();

        var longUsername = new string('a', UserValidation.MaxUsernameLength + 1);

        var authService = new AuthService(mockedUserRepository.Object, mockedPasswordHasher.Object,
            mockedJwtProvider.Object);

        var response = await authService.Register(new(longUsername, "pass", "Name"));

        var failure = Assert.IsType<RegisterUserCommand.Response.InvalidRequest>(response);

        Assert.Equal("Username is too long", failure.Message);

        mockedUserRepository.Verify(repo => repo.Add(It.IsAny<CreateUserQuery>()), Times.Never);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnFailure_WhenNameIsTooLong()
    {
        var mockedUserRepository = new Mock<IUserRepository>();
        var mockedPasswordHasher = new Mock<IPasswordHasher>();
        var mockedJwtProvider = new Mock<IJwtProvider>();

        var longName = new string('a', UserValidation.MaxNameLength + 1);

        var authService = new AuthService(mockedUserRepository.Object, mockedPasswordHasher.Object,
            mockedJwtProvider.Object);

        var response = await authService.Register(new("username", "pass", longName));

        var failure = Assert.IsType<RegisterUserCommand.Response.InvalidRequest>(response);

        Assert.Equal("Name is too long", failure.Message);

        mockedUserRepository.Verify(repo => repo.Add(It.IsAny<CreateUserQuery>()), Times.Never);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnFailure_WhenUserAlreadyExists()
    {
        var mockedUserRepository = new Mock<IUserRepository>();
        var mockedPasswordHasher = new Mock<IPasswordHasher>();
        var mockedJwtProvider = new Mock<IJwtProvider>();

        const string username = "username";

        mockedUserRepository
            .Setup(repo => repo.FindByUsername(username))
            .ReturnsAsync(new User(1, username, "Name"));

        var authService = new AuthService(mockedUserRepository.Object, mockedPasswordHasher.Object,
            mockedJwtProvider.Object);

        var response = await authService.Register(new(username, "pass", "OtherName"));

        var failure = Assert.IsType<RegisterUserCommand.Response.UserAlreadyExists>(response);

        Assert.Equal("User with this username already exists", failure.Message);

        mockedUserRepository.Verify(repo => repo.FindByUsername(username), Times.Once);

        mockedUserRepository.Verify(repo => repo.Add(It.IsAny<CreateUserQuery>()), Times.Never);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnFailure_WhenRepositoryThrowsException()
    {
        var mockedUserRepository = new Mock<IUserRepository>();
        var mockedPasswordHasher = new Mock<IPasswordHasher>();
        var mockedJwtProvider = new Mock<IJwtProvider>();
        
        mockedUserRepository
            .Setup(repo => repo.Add(It.IsAny<CreateUserQuery>()))
            .ThrowsAsync(new Exception("Database error"));

        var authService = new AuthService(mockedUserRepository.Object, mockedPasswordHasher.Object,
            mockedJwtProvider.Object);

        var response = await authService.Register(new("username", "pass", "Name"));

        var failure = Assert.IsType<RegisterUserCommand.Response.Failure>(response);

        Assert.Equal("Unexpected error while creating user", failure.Message);
    }
}