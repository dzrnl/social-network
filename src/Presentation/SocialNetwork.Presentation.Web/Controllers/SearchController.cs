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
    public async Task<IActionResult> Search(string? query, int page = 1)
    {
        GetUsersCommand.Response response;

        query = query?.Trim().ToLower();

        if (string.IsNullOrEmpty(query))
        {
            query = null;
            response = await _userService.GetUsers(new GetUsersCommand.Request(page, PageSize));
        }
        else
        {
            response = await _userService.GetUsers(new GetUsersCommand.Request(page, PageSize, query));
        }

        var success = (GetUsersCommand.Response.Success)response;

        var users = success.Users
            .Select(UserModel.ToViewModel)
            .ToList();

        var searchResult = new SearchUsersModel(page, PageSize, users, query);

        return View(searchResult);
    }
}