using Microsoft.EntityFrameworkCore;
using ToDoListManagement.Entity.Data;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Repository.Interfaces;

namespace ToDoListManagement.Repository.Implementations;

public class SprintRepository : ISprintRepository
{
    private readonly ToDoListDbContext _context;

    public SprintRepository(ToDoListDbContext context)
    {
        _context = context;
    }

    public async Task<List<Sprint>> GetAllAsync()
    {
        return await _context.Set<Sprint>().ToListAsync();
    }

    public async Task<Sprint?> GetByIdAsync(int id)
    {
        return await _context.Set<Sprint>().FindAsync(id);
    }

    public async Task<bool> AddAsync(Sprint entity)
    {
        await _context.Set<Sprint>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateAsync(Sprint entity)
    {
        _context.Set<Sprint>().Update(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Sprint>> GetSprintsByProjectId(int projectId)
    {
        return await _context.Set<Sprint>().Where(s => s.ProjectId == projectId).ToListAsync();
    }

    public async Task<Sprint?> GetSprintByNameAsync(string sprintName, int projectId)
    {
        return await _context.Set<Sprint>().FirstOrDefaultAsync(s => s.SprintName != null && s.SprintName.ToLower() == sprintName && s.ProjectId == projectId);
    }
}