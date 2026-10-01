using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MVC.Filters;

public class RoleAuthorizationFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(
        ActionExecutingContext context)
    {
        string role =
            context.HttpContext.Session.GetString("Role");

        string controllerName =
            context.RouteData.Values["controller"]?.ToString();

        if (string.IsNullOrEmpty(role))
        {
            context.Result = new RedirectToActionResult(
                "Login",
                "Home",
                null);

            return;
        }

        if (role == "User" &&
            controllerName != "User" &&
            controllerName != "Task")
        {
            context.Result = new RedirectToActionResult(
                "Index",
                "Task",
                null);

            return;
        }

        if (role == "Admin" &&
            controllerName != "Admin")
        {
            context.Result = new RedirectToActionResult(
                "Index",
                "Admin",
                null);

            return;
        }
    }
}