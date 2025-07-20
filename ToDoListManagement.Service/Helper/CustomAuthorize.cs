using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace ToDoListManagement.Service.Helper;

public class CustomAuthorize : Attribute, IAuthorizationFilter
{
    private readonly string[] _permission;
    private readonly string _action;

    public CustomAuthorize(string[] permission, string action = "CanView")
    {
        _permission = permission;
        _action = action;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        ClaimsPrincipal? user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        IGetPermissionService? permissionService = context.HttpContext.RequestServices.GetService<IGetPermissionService>();

        bool hasAnyPermission = _permission.Any(p =>
            permissionService != null && permissionService.HasPermission(user, p, _action));

        if (!hasAnyPermission)
        {
            context.Result = new ForbidResult();
        }
    }

    public class PermissionClaim
    {
        public string PermissionName { get; set; } = string.Empty;
        public bool CanView { get; set; }
        public bool CanAddEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}
