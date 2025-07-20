using Microsoft.EntityFrameworkCore;
using ToDoListManagement.Entity.Data;
using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Repository.Interfaces;

namespace ToDoListManagement.Repository.Implementations;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ToDoListDbContext _context;
    public EmployeeRepository(ToDoListDbContext context)
    {
        _context = context;
    }

    public async Task<Pagination<User>> GetPaginatedEmployeesAsync(Pagination<User> pagination, int userId)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        IQueryable<User> query = _context.Users.Include(r => r.Role)
            .Where(u => u.UserId != userId && !u.IsDeleted).AsQueryable();

        if (user != null && user.RoleId == 2)
        {
            query = query.Where(u => u.RoleId != 1);
        }

        if (!string.IsNullOrEmpty(pagination.SearchKeyword))
        {
            query = query.Where(p => (!string.IsNullOrEmpty(p.Name) && p.Name.ToLower().Contains(pagination.SearchKeyword.Trim().ToLower())) ||
            (!string.IsNullOrEmpty(p.Email) && p.Email.ToLower().Contains(pagination.SearchKeyword.Trim().ToLower())));
        }

        query = pagination.SortColumn switch
        {
            "email" => (pagination.SortDirection?.ToLower() ?? "asc") == "asc"
                                ? query.OrderBy(p => p.Email)
                                : query.OrderByDescending(p => p.Email),

            "role" => (pagination.SortDirection?.ToLower() ?? "asc") == "asc"
                                ? query.OrderBy(p => p.Role)
                                : query.OrderByDescending(p => p.Role),

            _ => pagination.SortDirection?.ToLower() == "asc"
                                ? query.OrderBy(p => p.Name)
                                : query.OrderByDescending(p => p.Name)
        };

        List<User> pagedData = await query
            .Skip((pagination.CurrentPage - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        int totalRecords = await query.CountAsync();
        int totalPages = (int)Math.Ceiling((double)totalRecords / pagination.PageSize);

        return new Pagination<User>
        {
            Items = pagedData,
            CurrentPage = pagination.CurrentPage,
            TotalPages = totalPages,
            TotalRecords = totalRecords
        };
    }

    public async Task<List<Role>> GetRolesForEmployeeAsync()
    {
        return await _context.Roles
            .Where(r => !r.IsDeleted)
            .OrderBy(r => r.RoleName)
            .ToListAsync();
    }

    public async Task<bool> AddEmployeeAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<User> GetEmployeeByIdAsync(int employeeId)
    {
        return await _context.Users
            .FirstAsync(u => u.UserId == employeeId && !u.IsDeleted);
    }

    public async Task<bool> UpdateEmployeeAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteEmployeeAsync(int employeeId)
    {
        User? existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == employeeId && !u.IsDeleted);
        if (existingUser == null)
        {
            return false;
        }

        existingUser.IsDeleted = true;
        _context.Users.Update(existingUser);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<User>> GetEmployeesByRoleIdAsync(int roleId)
    {
        return await _context.Users
            .Where(u => u.RoleId == roleId && !u.IsDeleted)
            .ToListAsync();
    }

    public async Task<int> GetCountOfEmployeesAsync()
    {
        return await _context.Users.CountAsync(u => u.RoleId != 1 && !u.IsDeleted);
    }

    public async Task<List<User>> GetNotAssignedUsersAsync()
    {
        return await _context.Users
                    .Where(u => u.RoleId != 1 && !u.IsDeleted && !_context.TeamUserMappings.Any(tum => tum.UserId == u.UserId && !tum.IsDeleted))
                    .Include(u => u.Role).ToListAsync();
    }
}