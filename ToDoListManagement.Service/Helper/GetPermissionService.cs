using System.Security.Claims;
using System.Text.Json;
using static ToDoListManagement.Service.Helper.CustomAuthorize;

namespace ToDoListManagement.Service.Helper;

public class GetPermissionService : IGetPermissionService
{
    public List<PermissionClaim> GetUserPermissions(ClaimsPrincipal user)
    {
        string? permissionsJson = user.FindFirst("Permissions")?.Value;
        return string.IsNullOrEmpty(permissionsJson)
            ? []
            : JsonSerializer.Deserialize<List<PermissionClaim>>(permissionsJson) ?? [];
    }

    public bool HasPermission(ClaimsPrincipal user, string permissionName, string action)
    {
        List<PermissionClaim>? permissions = GetUserPermissions(user);
        PermissionClaim? perm = permissions.FirstOrDefault(p => p.PermissionName == permissionName);

        return action switch
        {
            "CanView" => perm?.CanView ?? false,
            "CanAddEdit" => perm?.CanAddEdit ?? false,
            "CanDelete" => perm?.CanDelete ?? false,
            _ => false
        };
    }
}