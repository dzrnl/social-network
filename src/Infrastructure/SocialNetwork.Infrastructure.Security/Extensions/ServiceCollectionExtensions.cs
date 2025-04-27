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

        collection.Configure<TokenOptions>(configuration.GetSection(nameof(TokenOptions)));

        collection.AddScoped<IPasswordHasher, PasswordHasher>();

        collection.AddScoped<IJwtProvider, JwtProvider>();

        var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();
        var tokenOptions = configuration.GetSection(nameof(TokenOptions)).Get<TokenOptions>();

        if (jwtOptions != null && tokenOptions != null)
        {
            AddApiAuthentication(collection, jwtOptions, tokenOptions);
        }

        return collection;
    }

    private static void AddApiAuthentication(this IServiceCollection collection, JwtOptions jwtOptions,
        TokenOptions tokenOptions)
    {
        collection.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;

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
                    OnMessageReceived = context => {
                        context.Token = context.Request.Cookies[tokenOptions.AccessTokenCookieName];

                        return Task.CompletedTask;
                    }
                };
            });

        collection.AddAuthorization();
    }
}