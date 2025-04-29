using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SocialNetwork.Application.Contracts.Commands.Users;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Services;
using SocialNetwork.Infrastructure.Security;
using SocialNetwork.Presentation.Web.Models.Users;

namespace SocialNetwork.Presentation.Web.Controllers;

[Route("settings")]
public class SettingsController : BaseController
{
    private readonly IUserService _userService;
    private readonly IOptions<TokenOptions> _tokenOptions;

    public SettingsController(
        CurrentUserManager currentUserManager, 
        IUserService userService,
        IOptions<TokenOptions> tokenOptions) : base(currentUserManager)
    {
        _userService = userService;
        _tokenOptions = tokenOptions;
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
        
        if (response is ChangeUserNameCommand.Response.InvalidRequest invalidRequest)
        {
            ModelState.AddModelError(string.Empty, invalidRequest.Message);
            return View("Settings");
        }

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
        
        var cookieName = _tokenOptions.Value.AccessTokenCookieName;

        HttpContext.Response.Cookies.Delete(cookieName);

        return Redirect("/");
    }
}