using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Services;

namespace SocialNetwork.Presentation.Web.Controllers;

public class FeedController : BaseController
{
    public FeedController(CurrentUserManager currentUserManager) : base(currentUserManager) { }

    [HttpGet("/feed")]
    public IActionResult Feed()
    {
        return View();
    }
}