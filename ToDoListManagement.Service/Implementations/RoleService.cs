using ToDoListManagement.Entity.Models;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Repository.Interfaces;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Service.Implementations;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionService _permissionService;
    private readonly IEmployeeService _employeeService;
    public RoleService(IRoleRepository roleRepository, IPermissionService permissionService, IEmployeeService employeeService)
    {
        _roleRepository = roleRepository;
        _permissionService = permissionService;
        _employeeService = employeeService;
    }

    public async Task<List<RoleViewModel>> GetAllRoles()
    {
        List<Role> roles = await _roleRepository.GetAllRoles();

        List<RoleViewModel> roleViewModels = [];
        foreach (Role? role in roles)
        {
            roleViewModels.Add(new()
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName
            });
        }
        return roleViewModels;
    }

    public async Task<RoleViewModel> AddRoleAsync(RoleViewModel model, int userId)
    {
        Role role = new()
        {
            RoleName = model.RoleName?.Trim(),
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        Role roleWithId = await _roleRepository.AddRoleAsync(role);
        RoleViewModel roleViewModel = new(){
            RoleId = roleWithId.RoleId,
            RoleName = roleWithId.RoleName
        };
        if(roleWithId != null)
        {
            await _permissionService.AddPermissionsAsync(roleWithId.RoleId, userId);
        }
        return roleViewModel;
    }
    
    public async Task<bool> CheckRoleNameExistsAsync(string roleName, int roleId = 0)
    {
        Role? role = await _roleRepository.GetRoleByNameAsync(roleName.Trim().ToLower());
        if(role != null)
        {
           if(role.RoleId != roleId)
           {
               return true;
           }
           else
           {
               return false;
           }
        }
        return false;
    }

    public async Task<RoleViewModel> GetRoleByIdAsync(int roleId)
    {
        Role? role = await _roleRepository.GetRoleByIdAsync(roleId);

        if (role == null)
        {
            return new RoleViewModel();
        }

        return new RoleViewModel()
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName
        };
    }

    public async Task<bool> UpdateRoleAsync(RoleViewModel model, int userId)
    {
        Role? role = await _roleRepository.GetRoleByIdAsync(model.RoleId);

        if (role != null)
        {
            role.RoleName = model.RoleName?.Trim();
            role.UpdatedAt = DateTime.UtcNow;
            role.UpdatedBy = userId;
        }
        else
        {
            return false;
        }

        return await _roleRepository.UpdateRoleAsync(role);
    }

    public async Task<bool> DeleteRoleAsync(int roleId, int userId)
    {
        Role? role = await _roleRepository.GetRoleByIdAsync(roleId);

        if (role != null)
        {
            role.IsDeleted = true;
            role.UpdatedAt = DateTime.UtcNow;
            role.UpdatedBy = userId;
        }
        else
        {
            return false;
        }

        bool isDeleted = await _roleRepository.UpdateRoleAsync(role);
        if(isDeleted)
        {
            await _permissionService.DeletePermissionsAsync(roleId, userId);
        }

        return true;
    }

    public async Task<bool> CheckUsersRoleExists(int roleId)
    {
        List<EmployeeViewModel> users = await _employeeService.GetEmployeesByRoleAsync(roleId);
        return users.Count != 0;
    }
}