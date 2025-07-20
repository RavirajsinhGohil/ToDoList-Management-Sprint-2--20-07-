using ToDoListManagement.Entity.ViewModel;

namespace ToDoListManagement.Service.Interfaces;

public interface ITaskService
{
    Task<ProjectTasksViewModel> GetProjectNamesAsync(UserViewModel user);
    Task<List<TaskViewModel>?> GetBacklogTasksByProjectIdAsync(int projectId);
    Task<List<TaskViewModel>?> GetTasksBySprintIdAsync(int sprintId, int userId);
    Task<bool> UpdateTaskStatusAsync(int taskId, string newStatus);
    Task<bool> UpdateTaskAsync(TaskViewModel model, int userId);
    Task<List<UserViewModel>> GetTeamMembersAsync(int projectId);
    Task<bool> AddTaskAsync(TaskViewModel model, UserViewModel user);
    Task<TaskViewModel?> GetTaskByIdAsync(int taskId);
    Task<bool> CheckTaskNameExists(string projectName, int taskId, int projectId);
    Task<(FileStream? Stream, string? FileName)> DownloadFileAsync(int taskAttachmentId);
    Task<bool> DeleteTaskAsync(int taskId);
}
