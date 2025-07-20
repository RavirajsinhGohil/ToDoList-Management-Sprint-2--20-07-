using Microsoft.EntityFrameworkCore;
using ToDoListManagement.Entity.Data;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Repository.Interfaces;

namespace ToDoListManagement.Repository.Implementations;

public class TaskRepository : ITaskRepository
{
    private readonly ToDoListDbContext _context;

    public TaskRepository(ToDoListDbContext context)
    {
        _context = context;
    }

    public async Task<List<ToDoList>> GetTasksBySprintIdAsync(int sprintId)
    {
        return await _context.ToDoLists.Where(t => t.SprintId == sprintId && !t.IsDeleted).ToListAsync();
    }

    public async Task<List<ToDoList>> GetBacklogTasksByProjectIdAsync(int projectId)
    {
        return await _context.ToDoLists
            .Where(t => t.ProjectId == projectId && t.Status == "Not Started" && !t.IsDeleted)
            .Include(t => t.Project)
            .Include(t => t.Sprint)
            .OrderBy(t => t.Title)
            .ToListAsync();
    }

    public async Task<ToDoList?> GetTaskByTitleAsync(string title, int projectId)
    {
        return await _context.ToDoLists
            .Where(t => t.Title != null && t.Title.ToLower() == title && t.ProjectId == projectId && !t.IsDeleted)
            .Include(t => t.Project)
            .FirstOrDefaultAsync();
    }

    public async Task<ToDoList?> GetTaskByIdAsync(int taskId)
    {
        return await _context.ToDoLists.Include(t => t.Project).FirstOrDefaultAsync(t => t.TaskId == taskId);
    }

    public async Task<bool> UpdateTaskAsync(ToDoList task)
    {
        _context.ToDoLists.Update(task);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ProjectUser>> GetTeamMembersAsync(int projectId)
    {
        return await _context.ProjectUsers.Where(pu => pu.ProjectId == projectId && pu.User != null && pu.User.RoleId == 3 && !pu.User.IsDeleted).Include(pu => pu.User).ToListAsync();
    }

    public async Task<ToDoList> AddTaskAsync(ToDoList task)
    {
        await _context.ToDoLists.AddAsync(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<bool> AddAttachmentAsync(TaskAttachment taskAttachment)
    {
        await _context.TaskAttachments.AddAsync(taskAttachment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<TaskAttachment>> GetTaskAttachmentsByTaskIdAsync(int taskId)
    {
        return await _context.TaskAttachments.Where(ta => ta.TaskId == taskId && !ta.IsDeleted).ToListAsync();
    }

    public async Task<TaskAttachment?> GetTaskAttachmentAsync(int taskAttachmentId)
    {
        return await _context.TaskAttachments
            .FirstOrDefaultAsync(a => a.AttachmentId == taskAttachmentId && !a.IsDeleted);
    }

    public async Task<bool> DeleteTaskAttachmentAsync(TaskAttachment taskAttachment)
    {
        TaskAttachment? attachment = await GetTaskAttachmentAsync(taskAttachment.AttachmentId);
        if (attachment == null) return false;

        attachment = taskAttachment;
        _context.TaskAttachments.Update(attachment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetCountOfTasksAsync(int userId)
    {
        return await _context.ToDoLists.CountAsync(t => t.AssignedTo == userId && !t.IsDeleted);
    }
}