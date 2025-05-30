using SocialNetwork.Application.Contracts.Commands.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Services;
using SocialNetwork.Presentation.Web.Models.Auth;
using SocialNetwork.Infrastructure.Security;
using SocialNetwork.Presentation.Web.Filters;

namespace SocialNetwork.Presentation.Web.Controllers;

public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    private readonly IOptions<TokenOptions> _tokenOptions;

    public AuthController(
        CurrentUserManager currentUserManager,
        IAuthService authService,
        IOptions<TokenOptions> tokenOptions)
        : base(currentUserManager)
    {
        _authService = authService;
        _tokenOptions = tokenOptions;
    }

    [HttpGet("/register")]
    public IActionResult Register()
    {
        if (CurrentUser != null)
        {
            return RedirectToAction("Profile", "Profile", new { username = CurrentUser.Username });
        }

        return View();
    }

    [HttpPost("/register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var response = await _authService.Register(new(model.Username, model.Password, model.Name, model.Surname));

        if (response is RegisterUserCommand.Response.UserAlreadyExists userAlreadyExists)
        {
            ModelState.AddModelError(string.Empty, userAlreadyExists.Message);
            return View(model);
        }

        if (response is RegisterUserCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        return RedirectToAction("Login");
    }

    [HttpGet("/login")]
    public IActionResult Login()
    {
        if (CurrentUser != null)
        {
            return RedirectToAction("Profile", "Profile", new { username = CurrentUser.Username });
        }

        return View();
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var response = await _authService.Login(new(model.Username, model.Password));

        if (response is LoginUserCommand.Response.NotFound notFound)
        {
            ModelState.AddModelError("Username", notFound.Message);
            return View(model);
        }

        if (response is LoginUserCommand.Response.InvalidCredentials invalidCredentials)
        {
            ModelState.AddModelError("Password", invalidCredentials.Message);
            return View(model);
        }

        if (response is LoginUserCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        var success = (LoginUserCommand.Response.Success)response;

        var token = success.Token;

        var cookieName = _tokenOptions.Value.AccessTokenCookieName;

        HttpContext.Response.Cookies.Append(cookieName, token);

        return Redirect("/");
    }

    [AuthorizeUser]
    [HttpPost("/logout")]
    public IActionResult Logout()
    {
        var cookieName = _tokenOptions.Value.AccessTokenCookieName;

        HttpContext.Response.Cookies.Delete(cookieName);

        return RedirectToAction("Login");
    }
}