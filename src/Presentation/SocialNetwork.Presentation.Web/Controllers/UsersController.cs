using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Contracts.Commands.Users;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Services;
using SocialNetwork.Presentation.Web.Models.Users;

namespace SocialNetwork.Presentation.Web.Controllers;

public class UsersController : BaseController
{
    private readonly IUserService _userService;

    public UsersController(CurrentUserManager currentUserManager, IUserService userService) : base(currentUserManager)
    {
        _userService = userService;
    }

    [HttpGet("/{username}")]
    public async Task<IActionResult> Profile(string username)
    {
        var currentUser = CurrentUserManager.CurrentUser;

        if (currentUser?.Id == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var response = await _userService.GetUserByUsername(new(username));

        if (response is GetUserCommand.Response.NotFound)
        {
            return NotFound();
        }

        var user = ((GetUserCommand.Response.Success)response).User;

        var userModel = UserModel.ToViewModel(user);

        if (currentUser.Username != user.Username)
        {
            return View("ProfileView", userModel);
        }

        return View(userModel);
    }

    [HttpPost("/change-username")]
    public async Task<IActionResult> ChangeUserName(ChangeUserNameModel model)
    {
        var currentUser = CurrentUserManager.CurrentUser;

        if (currentUser == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var response = await _userService.ChangeUserName(new(currentUser.Id, model.NewName));

        if (response is ChangeUserNameCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        return RedirectToAction("Profile", "Users", new { username = currentUser.Username });
    }

    [HttpPost("/delete-account")]
    public async Task<IActionResult> DeleteUser()
    {
        var currentUserId = CurrentUserManager.CurrentUser?.Id;

        if (!currentUserId.HasValue)
        {
            return RedirectToAction("Login", "Auth");
        }

        var response = await _userService.DeleteUser(new(currentUserId.Value));

        if (response is DeleteUserCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        return Redirect("/");
    }
}