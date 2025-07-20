using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.ViewModel;

namespace ToDoListManagement.Service.Interfaces;

public interface IEmployeeService
{
    Task<Pagination<EmployeeViewModel>> GetPaginatedEmployeesAsync(Pagination<EmployeeViewModel> pagination, UserViewModel user);
    Task<List<RoleViewModel>> GetRolesForEmployeeAsync();
    Task<bool> AddEmployeeAsync(EmployeeViewModel employee);
    Task<EditEmployeeViewModel> GetEmployeeByIdAsync(int employeeId);
    Task<bool> UpdateEmployeeAsync(EditEmployeeViewModel employee);
    Task<bool> DeleteEmployeeAsync(int employeeId);
    Task<List<EmployeeViewModel>> GetEmployeesByRoleAsync(int roleId);
}
