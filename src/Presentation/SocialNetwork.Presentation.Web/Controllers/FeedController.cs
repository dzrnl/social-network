using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Services;

namespace SocialNetwork.Presentation.Web.Controllers;

public class FeedController : Controller
{
    private readonly CurrentUserManager _currentUserManager;

    public FeedController(CurrentUserManager currentUserManager)
    {
        _currentUserManager = currentUserManager;
    }

    [HttpGet]
    public IActionResult Feed()
    {
        ViewBag.CurrentUser = _currentUserManager.CurrentUser;

        return View();
    }
}