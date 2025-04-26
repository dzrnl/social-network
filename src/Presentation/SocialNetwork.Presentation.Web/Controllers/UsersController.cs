using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Contracts.Commands.Users;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Presentation.Web.Contracts;

namespace SocialNetwork.Presentation.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetUsers([FromQuery] GetUsersRequest request)
    {
        var response = await _userService.GetUsers(new(request.Page, request.PageSize));

        if (response is GetUsersCommand.Response.InvalidRequest invalidRequest)
        {
            return BadRequest(invalidRequest.Message);
        }

        if (response is GetUsersCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        var success = (GetUsersCommand.Response.Success)response;

        return Ok(success.Users);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<UserResponse>> GetUser(long id)
    {
        var response = await _userService.GetUserById(new(id));

        if (response is GetUserCommand.Response.NotFound)
        {
            return NotFound();
        }

        if (response is GetUserCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        var success = (GetUserCommand.Response.Success)response;

        var userResponse = UserResponse.ToResponse(success.User);

        return Ok(userResponse);
    }

    [HttpPatch("{id:long}")]
    [Authorize] // TODO: get current user
    public async Task<ActionResult> ChangeUserName(long id, ChangeUserNameRequest request)
    {
        var response = await _userService.ChangeUserName(new(id, request.NewName));

        if (response is ChangeUserNameCommand.Response.InvalidRequest invalidRequest)
        {
            return BadRequest(invalidRequest.Message);
        }

        if (response is ChangeUserNameCommand.Response.NotFound)
        {
            return NotFound();
        }

        if (response is ChangeUserNameCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        return Ok();
    }

    [HttpDelete("{id:long}")]
    [Authorize] // TODO: get current user
    public async Task<ActionResult> DeleteUser(long id)
    {
        var response = await _userService.DeleteUser(new(id));

        if (response is DeleteUserCommand.Response.NotFound)
        {
            return NotFound();
        }

        if (response is DeleteUserCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        return Ok();
    }
}