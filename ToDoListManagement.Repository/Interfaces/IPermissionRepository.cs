using ToDoListManagement.Entity.Models;

namespace ToDoListManagement.Repository.Interfaces;

public interface IPermissionRepository
{
    Task<List<Permission>> GetPermissionsByRoleAsync(int roleId);
    Task<bool> AddPermissionsAsync(List<Permission> permissions);
    Task<Permission?> GetPermissionByIdAsync(int permissionId);
    Task<bool> UpdatePermissionsAsync(Permission updatedPermission);
}