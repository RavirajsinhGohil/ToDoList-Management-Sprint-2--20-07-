using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Repository.Interfaces;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Service.Implementations;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<Pagination<EmployeeViewModel>> GetPaginatedEmployeesAsync(Pagination<EmployeeViewModel> pagination, UserViewModel user)
    {
        Pagination<User> employeePagination = new()
        {
            SearchKeyword = pagination.SearchKeyword,
            CurrentPage = pagination.CurrentPage,
            PageSize = pagination.PageSize,
            SortColumn = pagination.SortColumn,
            SortDirection = pagination.SortDirection
        };

        Pagination<User> employees = await _employeeRepository.GetPaginatedEmployeesAsync(employeePagination, user.UserId);

        List<EmployeeViewModel> employeeViewModels = [];
        foreach (User employee in employees.Items)
        {
            employeeViewModels.Add(new EmployeeViewModel
            {
                EmployeeId = employee.UserId,
                Name = employee.Name ?? string.Empty,
                Email = employee.Email ?? string.Empty,
                PhoneNumber = employee.PhoneNumber ?? string.Empty,
                Role = employee.RoleId,
                RoleName = employee.Role?.RoleName ?? string.Empty,
                Status = employee.IsActive ? "Active" : "Inactive"
            });
        }

        return new Pagination<EmployeeViewModel>
        {
            Items = employeeViewModels,
            TotalPages = employees.TotalPages,
            TotalRecords = employees.TotalRecords
        };
    }

    public async Task<List<RoleViewModel>> GetRolesForEmployeeAsync()
    {
        List<Role> roles = await _employeeRepository.GetRolesForEmployeeAsync();
        List<RoleViewModel> roleViewModels = [];
        foreach (Role role in roles)
        {
            roleViewModels.Add(new RoleViewModel
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName ?? string.Empty
            });
        }
        return roleViewModels;
    }

    public async Task<bool> AddEmployeeAsync(EmployeeViewModel employee)
    {
        User user = new()
        {
            Name = employee.Name?.Trim(),
            Email = employee.Email?.Trim().ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(employee.Password?.Trim()),
            PhoneNumber = employee.PhoneNumber,
            RoleId = employee.Role
        };
        return await _employeeRepository.AddEmployeeAsync(user);
    }

    public async Task<EditEmployeeViewModel> GetEmployeeByIdAsync(int employeeId)
    {
        User user = await _employeeRepository.GetEmployeeByIdAsync(employeeId);
        if (user == null)
        {
            return new EditEmployeeViewModel();
        }
        return new EditEmployeeViewModel
        {
            EmployeeId = user.UserId,
            Name = user.Name ?? string.Empty,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Role = user.RoleId,
            Status = user.IsActive ? "Active" : "Inactive",
            Roles = GetRolesForEmployeeAsync().Result
        };
    }

    public async Task<bool> UpdateEmployeeAsync(EditEmployeeViewModel employee)
    {
        User user = await _employeeRepository.GetEmployeeByIdAsync(employee.EmployeeId);
        if (user == null)
        {
            return false;
        }

        user.Name = employee.Name?.Trim();
        user.Email = employee.Email?.Trim().ToLower();
        user.PhoneNumber = employee.PhoneNumber;
        user.RoleId = employee.Role;
        user.IsActive = employee.Status == "Active";
        return await _employeeRepository.UpdateEmployeeAsync(user);
    }

    public async Task<bool> DeleteEmployeeAsync(int employeeId)
    {
        return await _employeeRepository.DeleteEmployeeAsync(employeeId);
    }

    public async Task<List<EmployeeViewModel>> GetEmployeesByRoleAsync(int roleId)
    {
        List<User> users = await _employeeRepository.GetEmployeesByRoleIdAsync(roleId);
        List<EmployeeViewModel> userViewModels = new();
        foreach (User user in users)
        {
            userViewModels.Add(new EmployeeViewModel()
            {
                EmployeeId = user.UserId,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                Role = user.RoleId,
                Email = user.Email,
                Status = user.IsActive ? "Active" : "Inactive"
            });
        }
        return userViewModels;
    }
}