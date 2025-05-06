using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Contracts.Commands.Users;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Services;
using SocialNetwork.Presentation.Web.Models.Users;

namespace SocialNetwork.Presentation.Web.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersApiController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly CurrentUserManager _currentUserManager;

    public UsersApiController(IUserService userService, CurrentUserManager userManager)
    {
        _userService = userService;
        _currentUserManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserModel>>> GetUsers([FromQuery] GetUsersModel model)
    {
        var response = await _userService.GetUsers(new(model.Page, model.PageSize));

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
    public async Task<ActionResult<UserModel>> GetUser(long id)
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

        var userResponse = UserModel.ToViewModel(success.User);

        return Ok(userResponse);
    }

    [HttpPatch("{id:long}/name")]
    [Authorize]
    public async Task<ActionResult> ChangeUserName(long id, ChangeUserNameModel model)
    {
        if (_currentUserManager.CurrentUser?.Id != id)
        {
            return Forbid();
        }

        var response = await _userService.ChangeUserName(new(id, model.NewName));

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
    
    [HttpPatch("{id:long}/surname")]
    [Authorize]
    public async Task<ActionResult> ChangeUserSurname(long id, ChangeUserSurnameModel model)
    {
        if (_currentUserManager.CurrentUser?.Id != id)
        {
            return Forbid();
        }

        var response = await _userService.ChangeUserSurname(new(id, model.NewSurname));

        if (response is ChangeUserSurnameCommand.Response.InvalidRequest invalidRequest)
        {
            return BadRequest(invalidRequest.Message);
        }

        if (response is ChangeUserSurnameCommand.Response.NotFound)
        {
            return NotFound();
        }

        if (response is ChangeUserSurnameCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        return Ok();
    }

    [HttpDelete("{id:long}")]
    [Authorize]
    public async Task<ActionResult> DeleteUser(long id)
    {
        if (_currentUserManager.CurrentUser?.Id != id)
        {
            return Forbid();
        }

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