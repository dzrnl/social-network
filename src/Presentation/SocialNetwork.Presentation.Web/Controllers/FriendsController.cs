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

    [HttpPost("add/{friendId:long}")]
    public async Task<IActionResult> AddFriend(long friendId, [FromForm] string? returnUrl)
    {
        var response = await _friendshipService.AddFriend(new(AuthUser.Id, friendId));

        if (response is AddFriendCommand.Response.Failure failure)
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