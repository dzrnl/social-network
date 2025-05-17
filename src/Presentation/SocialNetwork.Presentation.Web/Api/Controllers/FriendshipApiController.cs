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

        var response = await _friendshipService.SendFriendRequest(new(currentUserId, userId));

        return response switch
        {
            SendFriendRequestCommand.Response.SelfFriendship r => BadRequest(r.Message),
            SendFriendRequestCommand.Response.UserNotFound r => NotFound(r.Message),
            SendFriendRequestCommand.Response.AlreadyFriends r => BadRequest(r.Message),
            SendFriendRequestCommand.Response.RequestAlreadySent r => BadRequest(r.Message),
            SendFriendRequestCommand.Response.Failure r => StatusCode(500, r.Message),
            SendFriendRequestCommand.Response.Success => Ok(),
            _ => StatusCode(500, "Неизвестная ошибка")
        };
    }

    [HttpPost("remove/{userId:long}")]
    [Authorize]
    public async Task<ActionResult> RemoveFriend(long userId)
    {
        var currentUserId = _currentUserManager.CurrentUser!.Id;

        var response = await _friendshipService.RemoveFriend(new(currentUserId, userId));

        return response switch
        {
            RemoveFriendCommand.Response.UserNotFound r => NotFound(r.Message),
            RemoveFriendCommand.Response.NotFriends r => BadRequest(r.Message),
            RemoveFriendCommand.Response.Failure r => StatusCode(500, r.Message),
            RemoveFriendCommand.Response.Success => Ok(),
            _ => StatusCode(500, "Неизвестная ошибка")
        };
    }

    [HttpGet("status")]
    public async Task<ActionResult> GetFriendshipStatus([FromQuery] long user1Id, [FromQuery] long user2Id)
    {
        var response = await _friendshipService.AreFriends(new(user1Id, user2Id));

        return response switch
        {
            AreFriendsCommand.Response.UserNotFound r => NotFound(r.Message),
            AreFriendsCommand.Response.Failure r => StatusCode(500, r.Message),
            AreFriendsCommand.Response.Success s => Ok(s.AreFriends),
            _ => StatusCode(500, "Неизвестная ошибка")
        };
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