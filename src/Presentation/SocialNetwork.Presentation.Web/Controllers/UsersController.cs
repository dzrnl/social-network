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
        if (CurrentUser == null)
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

        if (CurrentUser.Username != user.Username)
        {
            return View("ProfileView", userModel);
        }

        return View(userModel);
    }

    [HttpPost("/change-username")]
    public async Task<IActionResult> ChangeUserName(ChangeUserNameModel model)
    {
        if (CurrentUser == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var response = await _userService.ChangeUserName(new(CurrentUser.Id, model.NewName));

        if (response is ChangeUserNameCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        return RedirectToAction("Profile", "Users", new { username = CurrentUser.Username });
    }

    [HttpPost("/delete-account")]
    public async Task<IActionResult> DeleteUser()
    {
        if (CurrentUser == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var response = await _userService.DeleteUser(new(CurrentUser.Id));

        if (response is DeleteUserCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        return Redirect("/");
    }
}