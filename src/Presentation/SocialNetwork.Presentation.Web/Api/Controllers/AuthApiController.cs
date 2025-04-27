using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SocialNetwork.Application.Contracts.Commands.Auth;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Infrastructure.Security;
using SocialNetwork.Presentation.Web.Contracts.Auth;

namespace SocialNetwork.Presentation.Web.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthApiController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IOptions<TokenOptions> _tokenOptions;

    public AuthApiController(IAuthService authService, IOptions<TokenOptions> tokenOptions)
    {
        _authService = authService;
        _tokenOptions = tokenOptions;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterUserRequest request)
    {
        var response = await _authService.Register(new(request.Username, request.Password, request.Name));

        if (response is RegisterUserCommand.Response.InvalidRequest invalidRequest)
        {
            return BadRequest(invalidRequest.Message);
        }

        if (response is RegisterUserCommand.Response.UserAlreadyExists userAlreadyExists)
        {
            return BadRequest(userAlreadyExists.Message);
        }

        if (response is RegisterUserCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        var success = (RegisterUserCommand.Response.Success)response;

        return Ok(success.Id);
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginUserRequest request)
    {
        var response = await _authService.Login(new(request.Username, request.Password));

        if (response is LoginUserCommand.Response.NotFound)
        {
            return NotFound();
        }

        if (response is LoginUserCommand.Response.InvalidCredentials invalidCredentials)
        {
            return Unauthorized(invalidCredentials.Message);
        }

        if (response is LoginUserCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        var success = (LoginUserCommand.Response.Success)response;

        var token = success.Token;

        var cookieName = _tokenOptions.Value.AccessTokenCookieName;

        HttpContext.Response.Cookies.Append(cookieName, token);

        return Ok();
    }
}