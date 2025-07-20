using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.Models;

namespace ToDoListManagement.Repository.Interfaces;

public interface IEmployeeRepository
{
    Task<Pagination<User>> GetPaginatedEmployeesAsync(Pagination<User> pagination, int userId);
    Task<List<Role>> GetRolesForEmployeeAsync();
    Task<bool> AddEmployeeAsync(User user);
    Task<User> GetEmployeeByIdAsync(int employeeId);
    Task<bool> UpdateEmployeeAsync(User user);
    Task<bool> DeleteEmployeeAsync(int employeeId);
    Task<List<User>> GetEmployeesByRoleIdAsync(int roleId);
    Task<int> GetCountOfEmployeesAsync();
    Task<List<User>> GetNotAssignedUsersAsync();
}
