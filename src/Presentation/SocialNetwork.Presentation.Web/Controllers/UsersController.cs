using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Contracts.Commands.Users;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Services;
using SocialNetwork.Presentation.Web.Models.Users;

namespace SocialNetwork.Presentation.Web.Controllers;

public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly CurrentUserManager _currentUserManager;

    public UsersController(IUserService userService, CurrentUserManager currentUserManager)
    {
        _userService = userService;
        _currentUserManager = currentUserManager;
    }

    [HttpGet("[controller]/[action]/{username}")]
    public async Task<IActionResult> Profile(string username)
    {
        var currentUser = _currentUserManager.CurrentUser;

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

    [HttpPost]
    public async Task<IActionResult> ChangeUserName(ChangeUserNameModel model)
    {
        var currentUserId = _currentUserManager.CurrentUser?.Id;

        if (!currentUserId.HasValue)
        {
            return RedirectToAction("Login", "Auth");
        }

        var response = await _userService.ChangeUserName(new(currentUserId.Value, model.NewName));

        if (response is ChangeUserNameCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        return RedirectToAction("Profile");
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser()
    {
        var currentUserId = _currentUserManager.CurrentUser?.Id;

        if (!currentUserId.HasValue)
        {
            return RedirectToAction("Login", "Auth");
        }

        var response = await _userService.DeleteUser(new(currentUserId.Value));

        if (response is DeleteUserCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        return RedirectToAction("Logout", "Auth");
    }
}