using System.Security.Claims;
using SocialNetwork.Application.Abstractions.Repositories;
using SocialNetwork.Application.Services;

namespace SocialNetwork.Presentation.Web.Middlewares;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, CurrentUserManager currentUserManager,
        IUserRepository userRepository)
    {
        var claims = httpContext.User.Claims.ToArray();

        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdClaim, out var userId))
            {
                var user = await userRepository.FindById(userId);
                
                currentUserManager.CurrentUser = user;
            }
        }

        await _next(httpContext);
    }
}