using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Services;
using SocialNetwork.Presentation.Web.Filters;

namespace SocialNetwork.Presentation.Web.Controllers;

[AuthorizeUser]
[Route("messages")]
public class MessagesController : BaseController
{
    public MessagesController(CurrentUserManager currentUserManager) : base(currentUserManager) { }

    [HttpGet]
    public IActionResult Messages()
    {
        return View();
    }
}