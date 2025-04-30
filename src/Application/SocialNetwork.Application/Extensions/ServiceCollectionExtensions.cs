using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Services;

namespace SocialNetwork.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection collection)
    {
        collection.AddScoped<CurrentUserManager>();

        collection.AddScoped<IAuthService, AuthService>();
        collection.AddScoped<IUserService, UserService>();
        collection.AddScoped<IFriendshipService, FriendshipService>();

        return collection;
    }
}