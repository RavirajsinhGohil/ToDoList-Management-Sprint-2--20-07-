using Microsoft.EntityFrameworkCore;
using ToDoListManagement.Entity.Data;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Repository.Interfaces;

namespace ToDoListManagement.Repository.Implementations;

public class PermissionRepository : IPermissionRepository
{
    private readonly ToDoListDbContext _context;

    public PermissionRepository(ToDoListDbContext context)
    {
        _context = context;
    }

    public async Task<List<Permission>> GetPermissionsByRoleAsync(int roleId)
    {
        return await _context.Permissions.Include(p => p.Role).Where(p => p.RoleId == roleId && !p.IsDeleted).OrderBy(p => p.PermissionName).ToListAsync();
    }

    public async Task<bool> AddPermissionsAsync(List<Permission> permissions)
    {
        await _context.Permissions.AddRangeAsync(permissions);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Permission?> GetPermissionByIdAsync(int permissionId)
    {
        return await _context.Permissions.FirstOrDefaultAsync(p => p.PermissionId == permissionId && !p.IsDeleted);
    }

    public async Task<bool> UpdatePermissionsAsync(Permission updatedPermission)
    {
        _context.Permissions.Update(updatedPermission);
        await _context.SaveChangesAsync();
        return true;
    }
}