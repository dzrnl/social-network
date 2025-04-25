using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.Application.Abstractions.Repositories;
using SocialNetwork.Infrastructure.DataAccess.Repositories;

namespace SocialNetwork.Infrastructure.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureDataAccess(this IServiceCollection collection,
        IConfiguration configuration)
    {
        collection.AddDbContext<SocialNetworkDbContext>(
            options => {
                options.UseNpgsql(configuration.GetConnectionString(nameof(SocialNetworkDbContext)))
                    .UseSnakeCaseNamingConvention();
            });

        collection.AddScoped<IUserRepository, UserRepository>();

        return collection;
    }
}