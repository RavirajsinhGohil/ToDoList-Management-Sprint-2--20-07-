using ToDoListManagement.Entity.Models;

namespace ToDoListManagement.Repository.Interfaces;

public interface ITaskRepository
{
    Task<List<ToDoList>> GetTasksBySprintIdAsync(int projectId);
    Task<List<ToDoList>> GetBacklogTasksByProjectIdAsync(int projectId);
    Task<ToDoList?> GetTaskByIdAsync(int taskId);
    Task<ToDoList?> GetTaskByTitleAsync(string title, int projectId);
    Task<bool> UpdateTaskAsync(ToDoList task);
    Task<List<ProjectUser>> GetTeamMembersAsync(int projectId);
    Task<ToDoList> AddTaskAsync(ToDoList task);
    Task<bool> AddAttachmentAsync(TaskAttachment taskAttachment);
    Task<List<TaskAttachment>> GetTaskAttachmentsByTaskIdAsync(int taskId);
    Task<TaskAttachment?> GetTaskAttachmentAsync(int taskAttachmentId);
    Task<bool> DeleteTaskAttachmentAsync(TaskAttachment taskAttachment);
    Task<int> GetCountOfTasksAsync(int userId);
}
