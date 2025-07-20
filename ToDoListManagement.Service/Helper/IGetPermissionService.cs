using System.Security.Claims;
using static ToDoListManagement.Service.Helper.CustomAuthorize;

namespace ToDoListManagement.Service.Helper;

public interface IGetPermissionService
{
    List<PermissionClaim> GetUserPermissions(ClaimsPrincipal user);
    bool HasPermission(ClaimsPrincipal user, string permissionName, string action);
}