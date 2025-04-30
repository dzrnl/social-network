using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Services;
using SocialNetwork.Presentation.Web.Filters;

namespace SocialNetwork.Presentation.Web.Controllers;

[AuthorizeUser]
[Route("friends")]
public class FriendsController : BaseController
{
    public FriendsController(CurrentUserManager currentUserManager) : base(currentUserManager) { }

    [HttpGet]
    public IActionResult Friends()
    {
        return View();
    }
}