using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Web.Controllers;

public class BaseController : Controller
{
    protected UserViewModel? SessionUser;
    private readonly IAuthService _authService;
    public BaseController(IAuthService authService)
    {
        _authService = authService;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        ClaimsIdentity? claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;

        if (claimsIdentity != null && HttpContext.User.Identity?.IsAuthenticated == true)
        {
            string? userIdStr = claimsIdentity.FindFirst("UserId")?.Value;

            if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))
            {
                SessionUser = new UserViewModel
                {
                    UserId = userId,
                    Name = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value,
                    Role = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value,
                    Email = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value,
                };
                ViewBag.SessionUser = SessionUser;
            }
        }
        await next();
    }
}