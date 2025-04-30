using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SocialNetwork.Presentation.Web.Controllers;

namespace SocialNetwork.Presentation.Web.Filters;

public class AuthorizeUserAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var controller = (BaseController)context.Controller;
        
        if (controller.CurrentUser == null)
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
        }
    }
}