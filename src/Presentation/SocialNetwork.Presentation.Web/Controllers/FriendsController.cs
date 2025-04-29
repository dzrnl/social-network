using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Services;

namespace SocialNetwork.Presentation.Web.Controllers;

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