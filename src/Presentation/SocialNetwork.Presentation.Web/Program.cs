using Microsoft.AspNetCore.CookiePolicy;
using SocialNetwork.Application.Extensions;
using SocialNetwork.Infrastructure.DataAccess.Extensions;
using SocialNetwork.Infrastructure.Security.Extensions;
using SocialNetwork.Presentation.Web.Middlewares;

namespace SocialNetwork.Presentation.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllersWithViews();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddApplication();
        builder.Services.AddInfrastructureDataAccess(builder.Configuration);
        builder.Services.AddInfrastructureSecurity(builder.Configuration);
        
        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        
        app.UseStaticFiles();

        app.UseCookiePolicy(new CookiePolicyOptions
        {
            MinimumSameSitePolicy = SameSiteMode.Strict,
            HttpOnly = HttpOnlyPolicy.Always,
            Secure = CookieSecurePolicy.Always
        });

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<AuthMiddleware>();

        app.MapControllers();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Feed}/{action=Feed}");

        app.Run();
    }
}