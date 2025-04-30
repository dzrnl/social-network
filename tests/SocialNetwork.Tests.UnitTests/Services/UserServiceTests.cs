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
    public async Task GetUsersTest()
    {
        var mockedUserRepository = new Mock<IUserRepository>();

        var expectedUsers = new List<User>
        {
            new(1, "ivanov123", "Ivan", "Ivanov"),
            new(2, "petrov12", "Peter", "Petrov")
        };

        const int page = 1;
        const int pageSize = 10;

        mockedUserRepository
            .Setup(repo => repo.FindPaged(It.Is<PaginationQuery>(q =>
                q.Page == page && q.PageSize == pageSize)))
            .ReturnsAsync(expectedUsers);

        var userService = new UserService(mockedUserRepository.Object);

        var response = await userService.GetUsers(new(page, pageSize));

        var success = Assert.IsType<GetUsersCommand.Response.Success>(response);

        Assert.Equal(expectedUsers, success.Users);

        mockedUserRepository.Verify(repo =>
            repo.FindPaged(It.Is<PaginationQuery>(q =>
                q.Page == page && q.PageSize == pageSize)
            ), Times.Once);
    }

    [Fact]
    public async Task GetUsers_ShouldReturnFailure_WhenPageIsLessThan1()
    {
        var mockedUserRepository = new Mock<IUserRepository>();

        var userService = new UserService(mockedUserRepository.Object);

        var response = await userService.GetUsers(new(0, 10));

        var failure = Assert.IsType<GetUsersCommand.Response.InvalidRequest>(response);

        Assert.Equal("Page number must be greater than or equal to 1", failure.Message);

        mockedUserRepository.Verify(repo => repo.FindPaged(It.IsAny<PaginationQuery>()), Times.Never);
    }

    [Fact]
    public async Task GetUsers_ShouldReturnFailure_WhenPageSizeIsLessThan1()
    {
        var mockedUserRepository = new Mock<IUserRepository>();

        var userService = new UserService(mockedUserRepository.Object);

        var response = await userService.GetUsers(new(1, 0));

        var failure = Assert.IsType<GetUsersCommand.Response.InvalidRequest>(response);

        Assert.Equal("Page size must be greater than or equal to 1", failure.Message);

        mockedUserRepository.Verify(repo => repo.FindPaged(It.IsAny<PaginationQuery>()), Times.Never);
    }

    [Fact]
    public async Task GetUsers_ShouldReturnFailure_WhenPageSizeExceedsMaxPageSize()
    {
        var mockedUserRepository = new Mock<IUserRepository>();

        var userService = new UserService(mockedUserRepository.Object);

        var response = await userService.GetUsers(new(1, UserService.MaxPageSize + 1));

        var failure = Assert.IsType<GetUsersCommand.Response.InvalidRequest>(response);

        Assert.Equal("Page size is too large", failure.Message);

        mockedUserRepository.Verify(repo => repo.FindPaged(It.IsAny<PaginationQuery>()), Times.Never);
    }

    [Fact]
    public async Task GetUsers_ShouldReturnFailure_WhenRepositoryThrowsException()
    {
        var mockedUserRepository = new Mock<IUserRepository>();

        mockedUserRepository
            .Setup(repo => repo.FindPaged(It.IsAny<PaginationQuery>()))
            .ThrowsAsync(new Exception("Database error"));

        var userService = new UserService(mockedUserRepository.Object);

        var response = await userService.GetUsers(new(1, 10));

        var failure = Assert.IsType<GetUsersCommand.Response.Failure>(response);

        Assert.Equal("Unexpected error while fetching users", failure.Message);
    }

    [Fact]
    public async Task GetUserByIdTest()
    {
        var mockedUserRepository = new Mock<IUserRepository>();

        const long userId = 1;
        User expectedUser = new(userId, "ivanov123", "Ivan", "Ivanov");

        mockedUserRepository
            .Setup(repo => repo.FindById(userId))
            .ReturnsAsync(expectedUser);

        var userService = new UserService(mockedUserRepository.Object);

        var response = await userService.GetUserById(new(userId));

        var success = Assert.IsType<GetUserCommand.Response.Success>(response);

        Assert.Equal(expectedUser, success.User);

        mockedUserRepository.Verify(repo => repo.FindById(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnFailure_WhenUserNotFound()
    {
        var mockedUserRepository = new Mock<IUserRepository>();

        const long userId = 1;

        mockedUserRepository
            .Setup(repo => repo.FindById(userId))
            .ReturnsAsync((User?)null);

        var userService = new UserService(mockedUserRepository.Object);

        var response = await userService.GetUserById(new(userId));

        Assert.IsType<GetUserCommand.Response.NotFound>(response);

        mockedUserRepository.Verify(repo => repo.FindById(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserById_ShouldReturnFailure_WhenRepositoryThrowsException()
    {
        var mockedUserRepository = new Mock<IUserRepository>();

        const long userId = 1;

        mockedUserRepository
            .Setup(repo => repo.FindById(userId))
            .ThrowsAsync(new Exception("Database error"));

        var userService = new UserService(mockedUserRepository.Object);

        var response = await userService.GetUserById(new GetUserCommand.Request.ById(userId));

        var failure = Assert.IsType<GetUserCommand.Response.Failure>(response);

        Assert.Equal("Unexpected error while fetching user", failure.Message);

        mockedUserRepository.Verify(repo => repo.FindById(userId), Times.Once);
    }

    [Fact]
    public async Task ChangeUserNameTest()
    {
        var mockedUserRepository = new Mock<IUserRepository>();

        const long userId = 1;
        const string newName = "Peter";

        mockedUserRepository
            .Setup(repo =>
                repo.ChangeName(It.Is<ChangeUserNameQuery>(q =>
                    q.Id == userId && q.NewName == newName)))
            .ReturnsAsync(true);

        var userService = new UserService(mockedUserRepository.Object);

        var response = await userService.ChangeUserName(new(userId, newName));

        Assert.IsType<ChangeUserNameCommand.Response.Success>(response);

        mockedUserRepository.Verify(repo =>
            repo.ChangeName(It.Is<ChangeUserNameQuery>(q =>
                q.Id == userId && q.NewName == newName)
            ), Times.Once);
    }

    [Fact]
    public async Task ChangeUserName_ShouldReturnInvalidRequest_WhenNameIsInvalid()
    {
        var mockedUserRepository = new Mock<IUserRepository>();

        const long userId = 1;
        const string invalidName = "";

        var userService = new UserService(mockedUserRepository.Object);

        var response = await userService.ChangeUserName(new(userId, invalidName));

        var failure = Assert.IsType<ChangeUserNameCommand.Response.InvalidRequest>(response);

        Assert.Equal("Name cannot be empty", failure.Message);

        mockedUserRepository.Verify(repo => repo.ChangeName(It.IsAny<ChangeUserNameQuery>()), Times.Never);
    }

    [Fact]
    public async Task ChangeUserName_ShouldReturnNotFound_WhenUserNotFound()
    {
        var mockedUserRepository = new Mock<IUserRepository>();

        const long userId = 1;
        const string newName = "Peter";

        mockedUserRepository
            .Setup(repo =>
                repo.ChangeName(It.Is<ChangeUserNameQuery>(q =>
                    q.Id == userId && q.NewName == newName)))
            .ReturnsAsync(false);

        var userService = new UserService(mockedUserRepository.Object);

        var response = await userService.ChangeUserName(new(userId, newName));

        Assert.IsType<ChangeUserNameCommand.Response.NotFound>(response);

        mockedUserRepository.Verify(repo =>
            repo.ChangeName(It.Is<ChangeUserNameQuery>(q =>
                q.Id == userId && q.NewName == newName)
            ), Times.Once);
    }

    [Fact]
    public async Task ChangeUserName_ShouldReturnFailure_WhenRepositoryThrowsException()
    {
        var mockedUserRepository = new Mock<IUserRepository>();

        const long userId = 1;
        const string newName = "Peter";

        mockedUserRepository
            .Setup(repo =>
                repo.ChangeName(It.Is<ChangeUserNameQuery>(q =>
                    q.Id == userId && q.NewName == newName)))
            .ThrowsAsync(new Exception("Database error"));

        var userService = new UserService(mockedUserRepository.Object);

        var response = await userService.ChangeUserName(new(userId, newName));

        var failure = Assert.IsType<ChangeUserNameCommand.Response.Failure>(response);

        Assert.Equal("Unexpected error while changing user name", failure.Message);

        mockedUserRepository.Verify(repo =>
            repo.ChangeName(It.Is<ChangeUserNameQuery>(q =>
                q.Id == userId && q.NewName == newName)
            ), Times.Once);
    }

    [Fact]
    public async Task DeleteUserTest()
    {
        var mockedUserRepository = new Mock<IUserRepository>();

        const long userId = 1;

        mockedUserRepository
            .Setup(repo => repo.Delete(userId))
            .ReturnsAsync(true);

        var userService = new UserService(mockedUserRepository.Object);

        var response = await userService.DeleteUser(new(userId));

        Assert.IsType<DeleteUserCommand.Response.Success>(response);

        mockedUserRepository.Verify(repo => repo.Delete(userId), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        var mockedUserRepository = new Mock<IUserRepository>();

        const long userId = 1;

        mockedUserRepository
            .Setup(repo => repo.Delete(userId))
            .ReturnsAsync(false);

        var userService = new UserService(mockedUserRepository.Object);

        var response = await userService.DeleteUser(new(userId));

        Assert.IsType<DeleteUserCommand.Response.NotFound>(response);

        mockedUserRepository.Verify(repo => repo.Delete(userId), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_ShouldReturnFailure_WhenRepositoryThrowsException()
    {
        var mockedUserRepository = new Mock<IUserRepository>();

        const long userId = 1;

        mockedUserRepository
            .Setup(repo => repo.Delete(userId))
            .ThrowsAsync(new Exception("Database failure"));

        var userService = new UserService(mockedUserRepository.Object);

        var response = await userService.DeleteUser(new(userId));

        var failure = Assert.IsType<DeleteUserCommand.Response.Failure>(response);

        Assert.Equal("Unexpected error while deleting user", failure.Message);

        mockedUserRepository.Verify(repo => repo.Delete(userId), Times.Once);
    }
}