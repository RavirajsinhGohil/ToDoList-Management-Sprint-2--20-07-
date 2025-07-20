using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using ToDoListManagement.Entity.Constants;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Repository.Interfaces;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Service.Implementations;

public class PermissionService : IPermissionService
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IServiceProvider _serviceProvider;
    public PermissionService(IPermissionRepository permissionRepository, IServiceProvider serviceProvider)
    {
        _permissionRepository = permissionRepository;
        _serviceProvider = serviceProvider;
    }

    public async Task<List<PermissionViewModel>> GetPermissionsByRoleAsync(int roleId)
    {
        List<Permission> permissions = await _permissionRepository.GetPermissionsByRoleAsync(roleId);
        if(permissions == null || permissions.Count == 0)
        {
            return null;
        }
        List<PermissionViewModel> permissionViewModels = [];
        foreach(var permission in permissions)
        {
            permissionViewModels.Add(new() {
                PermissionId = permission.PermissionId,
                PermissionName = permission.PermissionName,
                CanView = permission.CanView,
                CanAddEdit = permission.CanAddEdit,
                CanDelete = permission.CanDelete,
                RoleId = permission.RoleId,
                RoleName = permission.Role.RoleName
            });
        }

        return permissionViewModels;
    }

    public async Task<bool> AddPermissionsAsync(int roleId, int userId)
    {
        List<Permission> permissions = [];
        foreach(var tab in Constants.Tabs)
        {
            permissions.Add(new() {
                RoleId = roleId,
                PermissionName = tab,
                CanView = false,
                CanAddEdit = false,
                CanDelete = false,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            });
        }
        return await _permissionRepository.AddPermissionsAsync(permissions);
    }

    public async Task<bool> UpdatePermissionsAsync(List<PermissionViewModel> model, int userId)
    {
        foreach(PermissionViewModel? permission in model)
        {
            Permission? existingPermission = await _permissionRepository.GetPermissionByIdAsync(permission.PermissionId);
            if (existingPermission != null)
            {
                existingPermission.CanView = permission.CanView;
                existingPermission.CanAddEdit = permission.CanAddEdit;
                existingPermission.CanDelete = permission.CanDelete;
                existingPermission.UpdatedAt = DateTime.UtcNow;
                existingPermission.UpdatedBy = userId;
                await _permissionRepository.UpdatePermissionsAsync(existingPermission);
            }
        }
        IAuthService? authService = _serviceProvider.GetRequiredService<IAuthService>();
        await authService.RefreshUserPermissionsTokenAsync(userId);
        return true;
    }

    public async Task DeletePermissionsAsync(int roleId, int userId)
    {
        List<Permission> permissions = await _permissionRepository.GetPermissionsByRoleAsync(roleId);
        foreach (Permission permission in permissions)
        {
            permission.IsDeleted = true;
            permission.UpdatedAt = DateTime.UtcNow;
            permission.UpdatedBy = userId;
            await _permissionRepository.UpdatePermissionsAsync(permission);
        }
        IAuthService? authService = _serviceProvider.GetRequiredService<IAuthService>();
        await authService.RefreshUserPermissionsTokenAsync(userId);
    }
}