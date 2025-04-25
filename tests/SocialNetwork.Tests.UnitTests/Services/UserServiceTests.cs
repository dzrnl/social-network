using Moq;
using SocialNetwork.Application.Abstractions.Queries.Users;
using SocialNetwork.Application.Abstractions.Repositories;
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

        const string userName = "ivanov123";
        const long userId = 1;

        mockedUserRepository
            .Setup(repo => repo.Add(It.Is<CreateUserQuery>(q => q.Name == userName)))
            .ReturnsAsync(new User(userId, userName));

        var userService = new UserService(mockedUserRepository.Object);

        var userIdResult = await userService.CreateUser(userName);

        Assert.Equal(userId, userIdResult);

        mockedUserRepository.Verify(repo =>
            repo.Add(It.Is<CreateUserQuery>(q => q.Name == userName)), Times.Once);
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