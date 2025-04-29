using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SocialNetwork.Application.Models;
using SocialNetwork.Application.Services;

namespace SocialNetwork.Presentation.Web.Controllers;

public abstract class BaseController : Controller
{
    private readonly CurrentUserManager _currentUserManager;

    protected BaseController(CurrentUserManager currentUserManager)
    {
        _currentUserManager = currentUserManager;
    }
    
    protected User? CurrentUser => _currentUserManager.CurrentUser;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ViewData["CurrentUser"] = _currentUserManager.CurrentUser;
        base.OnActionExecuting(context);
    }
}
