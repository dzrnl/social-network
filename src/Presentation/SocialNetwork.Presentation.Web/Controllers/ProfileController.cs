using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Contracts.Commands.Friends;
using SocialNetwork.Application.Contracts.Commands.Users;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Services;
using SocialNetwork.Presentation.Web.Models.Profile;
using SocialNetwork.Presentation.Web.Models.Users;

namespace SocialNetwork.Presentation.Web.Controllers;

public class ProfileController : BaseController
{
    private readonly IUserService _userService;
    private readonly IFriendshipService _friendshipService;

    public ProfileController(
        CurrentUserManager currentUserManager,
        IUserService userService,
        IFriendshipService friendshipService)
        : base(currentUserManager)
    {
        _userService = userService;
        _friendshipService = friendshipService;
    }

    [HttpGet("/{username}")]
    public async Task<IActionResult> Profile(string username)
    {
        var response = await _userService.GetUserByUsername(new(username));

        if (response is GetUserCommand.Response.NotFound)
        {
            return NotFound();
        }

        var user = ((GetUserCommand.Response.Success)response).User;

        var userModel = UserModel.ToViewModel(user);

        if (CurrentUser == null)
        {
            var profileModel = new ProfileViewModel(userModel, false);

            return View("ProfileView", profileModel);
        }

        if (CurrentUser.Id != user.Id)
        {
            var responseAreFriends = await _friendshipService.AreFriends(new(CurrentUser.Id, user.Id));

            if (responseAreFriends is AreFriendsCommand.Response.Failure failure)
            {
                return UnprocessableEntity(failure.Message);
            }

            var isFriend = ((AreFriendsCommand.Response.Success)responseAreFriends).AreFriends;

            var profileModel = new ProfileViewModel(userModel, isFriend);

            return View("ProfileView", profileModel);
        }

        return View(userModel);
    }
}