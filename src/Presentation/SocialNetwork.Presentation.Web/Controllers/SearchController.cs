using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Contracts.Commands.Users;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Services;
using SocialNetwork.Presentation.Web.Models.Users;

namespace SocialNetwork.Presentation.Web.Controllers;

[Route("search")]
public class SearchController : BaseController
{
    private const int PageSize = 10;
    
    private readonly IUserService _userService;

    public SearchController(CurrentUserManager currentUserManager, IUserService userService) : base(currentUserManager)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Search(int page = 1)
    {
        var response = await _userService.GetUsers(new(page, PageSize));

        var success = (GetUsersCommand.Response.Success)response;

        var users = success.Users
            .Select(UserModel.ToViewModel)
            .ToList();

        var searchResult = new SearchUsersModel(page, PageSize, users);

        return View(searchResult);
    }
}