using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Contracts.Commands.Friends;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Services;
using SocialNetwork.Presentation.Web.Filters;
using SocialNetwork.Presentation.Web.Models.Friends;
using SocialNetwork.Presentation.Web.Models.Users;

namespace SocialNetwork.Presentation.Web.Controllers;

[AuthorizeUser]
[Route("friends")]
public class FriendsController : BaseController
{
    private const int PageSize = 10;

    private readonly IFriendshipService _friendshipService;

    public FriendsController(
        CurrentUserManager currentUserManager,
        IFriendshipService friendshipService) :
        base(currentUserManager)
    {
        _friendshipService = friendshipService;
    }

    [HttpGet]
    public async Task<IActionResult> Friends()
    {
        var response = await _friendshipService.GetUserFriends(new(AuthUser.Id, 1, PageSize));

        if (response is GetUserFriendsCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        var success = (GetUserFriendsCommand.Response.Success)response;

        var friends = success.Friends
            .Select(UserPreviewModel.ToViewModel)
            .ToList();

        var result = new UserFriendsModel(friends);

        return View(result);
    }

    [HttpGet("requests")]
    public async Task<IActionResult> IncomingRequests()
    {
        var response = await _friendshipService.GetUserIncomingRequests(new(AuthUser.Id));

        if (response is GetUserIncomingRequestsCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        var success = (GetUserIncomingRequestsCommand.Response.Success)response;

        var friendRequests = success.IncomingRequests
            .Select(FriendRequestModel.ToViewModel)
            .ToList();

        var result = new UserIncomingRequestsModel(friendRequests);

        return View("IncomingRequests", result);
    }

    [HttpGet("sent")]
    public async Task<IActionResult> SentRequests()
    {
        var response = await _friendshipService.GetUserSentRequests(new(AuthUser.Id));

        if (response is GetUserSentRequestsCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        var success = (GetUserSentRequestsCommand.Response.Success)response;

        var friendRequests = success.SentRequests
            .Select(FriendRequestModel.ToViewModel)
            .ToList();

        var result = new UserSentRequestsModel(friendRequests);

        return View("SentRequests", result);
    }

    [HttpPost("request/{friendId:long}")]
    public async Task<IActionResult> SendFriendRequest(long friendId, [FromForm] string? returnUrl)
    {
        var response = await _friendshipService.SendFriendRequest(new(AuthUser.Id, friendId));

        if (response is SendFriendRequestCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        return Redirect(returnUrl ?? "/");
    }

    [HttpPost("accept/{requestId:long}")]
    public async Task<IActionResult> AcceptFriendRequest(long requestId, [FromForm] string? returnUrl)
    {
        var response = await _friendshipService.AcceptFriendRequest(new(requestId));

        if (response is AcceptFriendRequestCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        return Redirect(returnUrl ?? "/");
    }

    [HttpPost("decline/{requestId:long}")]
    public async Task<IActionResult> DeclineFriendRequest(long requestId, [FromForm] string? returnUrl)
    {
        var response = await _friendshipService.DeclineFriendRequest(new(requestId));

        if (response is DeclineFriendRequestCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        return Redirect(returnUrl ?? "/");
    }

    [HttpPost("cancel/{requestId:long}")]
    public async Task<IActionResult> CancelFriendRequest(long requestId, [FromForm] string? returnUrl)
    {
        var response = await _friendshipService.CancelFriendRequest(new(requestId));

        if (response is CancelFriendRequestCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        return Redirect(returnUrl ?? "/");
    }

    [HttpPost("delete/{friendId:long}")]
    public async Task<IActionResult> DeleteFriend(long friendId, [FromForm] string? returnUrl)
    {
        var response = await _friendshipService.RemoveFriend(new(AuthUser.Id, friendId));

        if (response is RemoveFriendCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        return Redirect(returnUrl ?? "/");
    }
}