using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Contracts.Commands.Friends;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Services;
using SocialNetwork.Presentation.Web.Models.Friends;
using SocialNetwork.Presentation.Web.Models.Users;

namespace SocialNetwork.Presentation.Web.Api.Controllers;

[ApiController]
[Route("api/friends")]
public class FriendshipApiController : ControllerBase
{
    private readonly IFriendshipService _friendshipService;
    private readonly CurrentUserManager _currentUserManager;

    public FriendshipApiController(IFriendshipService friendshipService, CurrentUserManager currentUserManager)
    {
        _friendshipService = friendshipService;
        _currentUserManager = currentUserManager;
    }

    [HttpPost("add/{userId:long}")]
    [Authorize]
    public async Task<ActionResult> AddFriend(long userId)
    {
        var currentUserId = _currentUserManager.CurrentUser!.Id;

        var response = await _friendshipService.AddFriend(new(currentUserId, userId));

        if (response is AddFriendCommand.Response.SelfFriendship selfFriendship)
        {
            return BadRequest(selfFriendship.Message);
        }

        if (response is AddFriendCommand.Response.UserNotFound userNotFound)
        {
            return BadRequest(userNotFound.Message);
        }

        if (response is AddFriendCommand.Response.AlreadyFriends alreadyFriends)
        {
            return BadRequest(alreadyFriends.Message);
        }

        if (response is AddFriendCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        return Ok();
    }

    [HttpPost("remove/{userId:long}")]
    [Authorize]
    public async Task<ActionResult> RemoveFriend(long userId)
    {
        var currentUserId = _currentUserManager.CurrentUser!.Id;

        var response = await _friendshipService.RemoveFriend(new(currentUserId, userId));

        if (response is RemoveFriendCommand.Response.UserNotFound userNotFound)
        {
            return BadRequest(userNotFound.Message);
        }

        if (response is RemoveFriendCommand.Response.NotFriends notFriends)
        {
            return BadRequest(notFriends.Message);
        }

        if (response is RemoveFriendCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        return Ok();
    }

    [HttpGet("status")]
    public async Task<ActionResult> GetFriendshipStatus([FromQuery] long user1Id, [FromQuery] long user2Id)
    {
        var response = await _friendshipService.AreFriends(new(user1Id, user2Id));

        if (response is AreFriendsCommand.Response.UserNotFound userNotFound)
        {
            return BadRequest(userNotFound.Message);
        }

        if (response is AreFriendsCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        var success = (AreFriendsCommand.Response.Success)response;

        return Ok(success.AreFriends);
    }

    [HttpGet]
    public async Task<ActionResult> GetUserFriends([FromQuery] GetUserFriendsModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _friendshipService.GetUserFriends(new(model.UserId, model.Page, model.PageSize));

        if (response is GetUserFriendsCommand.Response.UserNotFound userNotFound)
        {
            return NotFound(userNotFound.Message);
        }

        if (response is GetUserFriendsCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        var success = (GetUserFriendsCommand.Response.Success)response;

        var userFriendsModel = new UserFriendsModel(success.Friends
            .Select(UserPreviewModel.ToViewModel)
            .ToList());

        return Ok(userFriendsModel);
    }
}