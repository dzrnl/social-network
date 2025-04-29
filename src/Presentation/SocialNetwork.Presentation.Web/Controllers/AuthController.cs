using SocialNetwork.Application.Contracts.Commands.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Services;
using SocialNetwork.Presentation.Web.Models.Auth;
using SocialNetwork.Infrastructure.Security;

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
        var currentUser = CurrentUserManager.CurrentUser;

        if (currentUser != null)
        {
            return RedirectToAction("Profile", "Users", new { username = currentUser.Username });
        }

        return View();
    }

    [HttpPost("/register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        var response = await _authService.Register(new(model.Username, model.Password, model.Name));

        if (response is RegisterUserCommand.Response.InvalidRequest invalidRequest)
        {
            ModelState.AddModelError(string.Empty, invalidRequest.Message);
            return View(model);
        }

        if (response is RegisterUserCommand.Response.UserAlreadyExists userAlreadyExists)
        {
            ModelState.AddModelError(string.Empty, userAlreadyExists.Message);
            return View(model);
        }

        if (response is RegisterUserCommand.Response.Failure failure)
        {
            ModelState.AddModelError(string.Empty, failure.Message);
            return View(model);
        }

        return RedirectToAction("Login");
    }

    [HttpGet("/login")]
    public IActionResult Login()
    {
        var currentUser = CurrentUserManager.CurrentUser;

        if (currentUser != null)
        {
            return RedirectToAction("Profile", "Users", new { username = currentUser.Username });
        }

        return View();
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        var response = await _authService.Login(new(model.Username, model.Password));

        if (response is LoginUserCommand.Response.NotFound notFound)
        {
            ModelState.AddModelError(string.Empty, notFound.Message);
            return View(model);
        }

        if (response is LoginUserCommand.Response.InvalidCredentials invalidCredentials)
        {
            ModelState.AddModelError(string.Empty, invalidCredentials.Message);
            return View(model);
        }

        if (response is LoginUserCommand.Response.Failure failure)
        {
            ModelState.AddModelError(string.Empty, failure.Message);
            return View(model);
        }

        var success = (LoginUserCommand.Response.Success)response;

        var token = success.Token;

        var cookieName = _tokenOptions.Value.AccessTokenCookieName;

        HttpContext.Response.Cookies.Append(cookieName, token);

        return RedirectToAction("Feed", "Feed");
    }
    
    [HttpPost("/logout")]
    public IActionResult Logout()
    {
        var cookieName = _tokenOptions.Value.AccessTokenCookieName;

        HttpContext.Response.Cookies.Delete(cookieName);

        return RedirectToAction("Login");
    }
}