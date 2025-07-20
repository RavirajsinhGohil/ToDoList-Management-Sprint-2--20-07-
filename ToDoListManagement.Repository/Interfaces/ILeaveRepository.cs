using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.Models;

namespace ToDoListManagement.Repository.Interfaces;

public interface ILeaveRepository : IBaseRepository<Leave>
{
    new Task<List<Leave>> GetAllAsync();
    new Task<Leave?> GetByIdAsync(int id);
    new Task<bool> AddAsync(Leave entity);
    new Task<bool> UpdateAsync(Leave entity);
    Task<Pagination<Leave>> GetPaginatedLeavesAsync(Pagination<Leave> pagination, int? userId);
    Task<int> GetCountOfLeavesAsync(int userId);
}