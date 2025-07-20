using ToDoListManagement.Entity.Models;

namespace ToDoListManagement.Repository.Interfaces;

public interface ISprintRepository : IBaseRepository<Sprint>
{
    new Task<List<Sprint>> GetAllAsync();
    new Task<Sprint?> GetByIdAsync(int id);
    new Task<bool> AddAsync(Sprint entity);
    new Task<bool> UpdateAsync(Sprint entity);
    Task<List<Sprint>> GetSprintsByProjectId(int projectId);
    Task<Sprint?> GetSprintByNameAsync(string sprintName, int projectId);
}