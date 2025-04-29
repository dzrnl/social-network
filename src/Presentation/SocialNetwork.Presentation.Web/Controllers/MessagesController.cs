using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Application.Services;

namespace SocialNetwork.Presentation.Web.Controllers;

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