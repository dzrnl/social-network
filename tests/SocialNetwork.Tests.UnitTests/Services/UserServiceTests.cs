using Moq;
using SocialNetwork.Application.Abstractions.Queries.Users;
using SocialNetwork.Application.Abstractions.Repositories;
using SocialNetwork.Application.Contracts.Commands.Users;
using SocialNetwork.Application.Models;
using SocialNetwork.Application.Services;
using Xunit;

namespace SocialNetwork.Tests.UnitTests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task CreateUserTest()
    {
        var mockedUserRepository = new Mock<IUserRepository>();

        const long userId = 1;
        const string userName = "ivanov123";

        mockedUserRepository
            .Setup(repo => repo.Add(It.Is<CreateUserQuery>(q => q.Name == userName)))
            .ReturnsAsync(new User(userId, userName));

        var userService = new UserService(mockedUserRepository.Object);

        var response = await userService.CreateUser(new(userName));

        var success = Assert.IsType<CreateUserCommand.Response.Success>(response);

        Assert.Equal(userId, success.Id);

        mockedUserRepository.Verify(repo =>
            repo.Add(It.Is<CreateUserQuery>(q => q.Name == userName)), Times.Once);
    }
    
    [Fact]
    public async Task CreateUser_ShouldReturnFailure_WhenNameIsEmpty()
    {
        var mockedUserRepository = new Mock<IUserRepository>();

        var userService = new UserService(mockedUserRepository.Object);

        var response = await userService.CreateUser(new(""));

        var failure = Assert.IsType<CreateUserCommand.Response.InvalidRequest>(response);
        
        Assert.Equal("Name cannot be empty", failure.Message);

        mockedUserRepository.Verify(repo => repo.Add(It.IsAny<CreateUserQuery>()), Times.Never);
    }
    
    [Fact]
    public async Task CreateUser_ShouldReturnFailure_WhenNameIsTooLong()
    {
        var mockedUserRepository = new Mock<IUserRepository>();

        var longUserName = new string('a', UserService.MaxNameLength + 1);

        var userService = new UserService(mockedUserRepository.Object);

        var response = await userService.CreateUser(new(longUserName));

        var failure = Assert.IsType<CreateUserCommand.Response.InvalidRequest>(response);

        Assert.Equal("Name is too long", failure.Message);

        mockedUserRepository.Verify(repo => repo.Add(It.IsAny<CreateUserQuery>()), Times.Never);
    }
    
    [Fact]
    public async Task CreateUser_ShouldReturnFailure_WhenRepositoryThrowsException()
    {
        var mockedUserRepository = new Mock<IUserRepository>();

        mockedUserRepository
            .Setup(repo => repo.Add(It.IsAny<CreateUserQuery>()))
            .ThrowsAsync(new Exception("Database error"));

        var userService = new UserService(mockedUserRepository.Object);

        var response = await userService.CreateUser(new("name"));

        var failure = Assert.IsType<CreateUserCommand.Response.Failure>(response);

        Assert.Equal("Unexpected error while creating user", failure.Message);
    }

    [Fact]
    public async Task GetUsersTest()
    {
        var mockedUserRepository = new Mock<IUserRepository>();

        var expectedUsers = new List<User> { new(1, "ivanov123"), new(2, "petrov12") };

        const int page = 1;
        const int pageSize = 10;

        mockedUserRepository
            .Setup(repo => repo.FindPaged(It.Is<PaginationQuery>(q => q.Page == page && q.PageSize == pageSize)))
            .ReturnsAsync(expectedUsers);

        var userService = new UserService(mockedUserRepository.Object);

        var actualUsers = await userService.GetUsers(page, pageSize);

        Assert.Equal(expectedUsers, actualUsers);

        mockedUserRepository.Verify(repo =>
            repo.FindPaged(It.Is<PaginationQuery>(q => q.Page == page && q.PageSize == pageSize)), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdTest()
    {
        var mockedUserRepository = new Mock<IUserRepository>();

        const long userId = 1;
        User expectedUser = new(userId, "ivanov123");

        mockedUserRepository
            .Setup(repo => repo.FindById(userId))
            .ReturnsAsync(expectedUser);

        var userService = new UserService(mockedUserRepository.Object);

        var actualUser = await userService.GetUserById(userId);

        Assert.NotNull(actualUser);

        Assert.Equal(expectedUser, actualUser);

        mockedUserRepository.Verify(repo => repo.FindById(userId), Times.Once);
    }

    [Fact]
    public async Task ChangeUserNameTest()
    {
        var mockedUserRepository = new Mock<IUserRepository>();

        User user = new(1, "ivanov123");
        const string newName = "petrov12";

        var userService = new UserService(mockedUserRepository.Object);

        await userService.ChangeUserName(user.Id, newName);

        mockedUserRepository.Verify(repo =>
            repo.ChangeUserName(It.Is<ChangeUserNameQuery>(q => q.Id == user.Id && q.Name == newName)), Times.Once);
    }
}