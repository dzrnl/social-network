using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Contracts.Commands.Auth;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Presentation.Web.Contracts;

namespace SocialNetwork.Presentation.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterUserRequest request)
    {
        var response = await _authService.Register(new(request.Username, request.Password, request.Name));

        if (response is RegisterUserCommand.Response.InvalidRequest invalidRequest)
        {
            return BadRequest(invalidRequest.Message);
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

        HttpContext.Response.Cookies.Append("access_token", token);

        return Ok();
    }
}