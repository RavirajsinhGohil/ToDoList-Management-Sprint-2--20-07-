using ToDoListManagement.Entity.ViewModel;

namespace ToDoListManagement.Service.Interfaces;

public interface IPermissionService
{
    Task<List<PermissionViewModel>> GetPermissionsByRoleAsync(int roleId);
    Task<bool> AddPermissionsAsync(int roleId, int userId);
    Task<bool> UpdatePermissionsAsync(List<PermissionViewModel> model, int userId);
    Task DeletePermissionsAsync(int roleId, int userId);
}
