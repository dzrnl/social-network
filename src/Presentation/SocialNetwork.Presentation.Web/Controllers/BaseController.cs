using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SocialNetwork.Application.Services;

namespace SocialNetwork.Presentation.Web.Controllers;

public abstract class BaseController : Controller
{
    protected readonly CurrentUserManager CurrentUserManager;

    protected BaseController(CurrentUserManager currentUserManager)
    {
        CurrentUserManager = currentUserManager;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ViewData["CurrentUser"] = CurrentUserManager.CurrentUser;
        base.OnActionExecuting(context);
    }
}
