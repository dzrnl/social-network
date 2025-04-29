using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Contracts.Commands.Users;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Services;
using SocialNetwork.Presentation.Web.Models.Users;

namespace SocialNetwork.Presentation.Web.Controllers;

[Route("settings")]
public class SettingsController : BaseController
{
    private readonly IUserService _userService;

    public SettingsController(CurrentUserManager currentUserManager, IUserService userService) : base(currentUserManager)
    {
        _userService = userService;
    }

    [HttpGet]
    public IActionResult Settings()
    {
        return View();
    }
    
    [HttpPost("change-username")]
    public async Task<IActionResult> ChangeUserName(ChangeUserNameModel model)
    {
        if (CurrentUser == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var response = await _userService.ChangeUserName(new(CurrentUser.Id, model.NewName));

        if (response is ChangeUserNameCommand.Response.Failure failure)
        {
            return UnprocessableEntity(failure.Message);
        }

        return RedirectToAction("Settings");
    }

    [HttpPost("delete-account")]
    public async Task<IActionResult> DeleteUser()
    {
        if (CurrentUser == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var response = await _userService.DeleteUser(new(CurrentUser.Id));

        if (response is DeleteUserCommand.Response.Failure failure)
        {
            return UnprocessableEntity(failure.Message);
        }

        return Redirect("/"); // TODO (del cookie)
    }
}