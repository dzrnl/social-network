using SocialNetwork.Application.Contracts.Commands.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Presentation.Web.Models.Auth;
using SocialNetwork.Infrastructure.Security;

namespace SocialNetwork.Presentation.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IOptions<TokenOptions> _tokenOptions;

    public AuthController(IAuthService authService, IOptions<TokenOptions> tokenOptions)
    {
        _authService = authService;
        _tokenOptions = tokenOptions;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
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

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
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

        return RedirectToAction("Profile", "Users");
    }

    [HttpPost]
    public IActionResult Logout()
    {
        var cookieName = _tokenOptions.Value.AccessTokenCookieName;

        HttpContext.Response.Cookies.Delete(cookieName);

        return RedirectToAction("Login", "Auth");
    }
}