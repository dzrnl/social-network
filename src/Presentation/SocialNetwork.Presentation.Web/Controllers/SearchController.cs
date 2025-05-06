using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Contracts.Commands.Users;
using SocialNetwork.Application.Contracts.Services;
using SocialNetwork.Application.Models;
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
        List<User> users;

        query = query?.Trim().ToLower();

        if (string.IsNullOrEmpty(query))
        {
            var response = await _userService.GetUsers(new(page, PageSize));

            if (response is GetUsersCommand.Response.InvalidRequest invalidRequest)
            {
                return BadRequest(invalidRequest);
            }

            if (response is GetUsersCommand.Response.Failure failure)
            {
                return UnprocessableEntity(failure.Message);
            }

            var success = (GetUsersCommand.Response.Success)response;

            users = success.Users;
        }
        else
        {
            var response = await _userService.SearchUsers(new(query, page, PageSize));

            if (response is SearchUsersCommand.Response.InvalidRequest invalidRequest)
            {
                return BadRequest(invalidRequest);
            }

            if (response is SearchUsersCommand.Response.Failure failure)
            {
                return UnprocessableEntity(failure.Message);
            }

            var success = (SearchUsersCommand.Response.Success)response;

            users = success.Users;
        }

        var usersModel = users
            .Select(UserModel.ToViewModel)
            .ToList();

        var searchResult = new SearchUsersModel(page, PageSize, usersModel, query);

        return View(searchResult);
    }
}