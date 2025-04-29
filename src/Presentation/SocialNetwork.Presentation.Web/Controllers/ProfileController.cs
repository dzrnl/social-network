using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Contracts.Commands.Users;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Services;
using SocialNetwork.Presentation.Web.Models.Users;

namespace SocialNetwork.Presentation.Web.Controllers;

public class ProfileController : BaseController
{
    private readonly IUserService _userService;

    public ProfileController(CurrentUserManager currentUserManager, IUserService userService) : base(currentUserManager)
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
}