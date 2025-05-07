using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SocialNetwork.Application.Contracts.Commands.Users;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Services;
using SocialNetwork.Infrastructure.Security;
using SocialNetwork.Presentation.Web.Filters;
using SocialNetwork.Presentation.Web.Models.Settings;

namespace SocialNetwork.Presentation.Web.Controllers;

[AuthorizeUser]
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
    public async Task<IActionResult> Settings()
    {
        var userResponse = await _userService.GetUserById(new(AuthUser.Id));

        var user = ((GetUserCommand.Response.Success)userResponse).User;

        var publicInfoModel = new UserPublicInfoModel
        {
            Name = user.Name,
            Surname = user.Surname
        };

        return View("Settings", publicInfoModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUserPublicInfo(UserPublicInfoModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Settings", model); 
        }
        
        if (model.Name != AuthUser.Name)
        {
            var response = await _userService.ChangeUserName(new(AuthUser.Id, model.Name));

            if (response is ChangeUserNameCommand.Response.Failure failure)
            {
                return StatusCode(500, failure.Message);
            }
        }

        if (model.Surname != AuthUser.Surname)
        {
            var response = await _userService.ChangeUserSurname(new(AuthUser.Id, model.Surname));

            if (response is ChangeUserSurnameCommand.Response.Failure failure)
            {
                return StatusCode(500, failure.Message);
            }
        }

        return RedirectToAction("Settings");
    }

    [HttpPost("delete-account")]
    public async Task<IActionResult> DeleteUser()
    {
        var response = await _userService.DeleteUser(new(AuthUser.Id));

        if (response is DeleteUserCommand.Response.Failure failure)
        {
            return StatusCode(500, failure.Message);
        }

        var cookieName = _tokenOptions.Value.AccessTokenCookieName;

        HttpContext.Response.Cookies.Delete(cookieName);

        return Redirect("/");
    }
}