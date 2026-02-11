using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BuildCore.WebApp.Filters;

/// <summary>
/// Login gerektiren action'lar için filter
/// </summary>
public class RequireLoginAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userId = context.HttpContext.Session.GetString("UserId");
        
        if (string.IsNullOrEmpty(userId))
        {
            // Login sayfasına yönlendir
            context.Result = new RedirectToActionResult("Login", "Auth", new { returnUrl = context.HttpContext.Request.Path });
            return;
        }

        base.OnActionExecuting(context);
    }
}
