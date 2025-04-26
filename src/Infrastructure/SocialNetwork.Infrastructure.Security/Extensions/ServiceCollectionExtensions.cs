using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SocialNetwork.Application.Abstractions.Auth;

namespace SocialNetwork.Infrastructure.Security.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureSecurity(this IServiceCollection collection,
        IConfiguration configuration)
    {
        collection.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

        collection.AddScoped<IPasswordHasher, PasswordHasher>();

        collection.AddScoped<IJwtProvider, JwtProvider>();
        
        var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();
        
        AddApiAuthentication(collection, jwtOptions);

        return collection;
    }

    public static void AddApiAuthentication(this IServiceCollection collection, JwtOptions jwtOptions)
    {
        collection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context => 
                    {
                        context.Token = context.Request.Cookies["access_token"];

                        return Task.CompletedTask;
                    }
                };
            });

        collection.AddAuthorization();
    }
}