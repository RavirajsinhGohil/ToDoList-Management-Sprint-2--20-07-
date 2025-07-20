using ToDoListManagement.Entity.ViewModel;

namespace ToDoListManagement.Service.Interfaces;

public interface IRoleService
{
    Task<List<RoleViewModel>> GetAllRoles();
    Task<RoleViewModel> AddRoleAsync(RoleViewModel model, int userId);
    Task<bool> CheckRoleNameExistsAsync(string roleName, int roleId);
    Task<RoleViewModel> GetRoleByIdAsync(int roleId);
    Task<bool> UpdateRoleAsync(RoleViewModel model, int userId);
    Task<bool> DeleteRoleAsync(int roleId, int userId);
    Task<bool> CheckUsersRoleExists(int roleId);
}