using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ToDoListManagement.Entity.Models;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Repository.Interfaces;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Service.Implementations;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectService _projectService;
    private readonly IWebHostEnvironment _environment;
    private readonly ISprintService _sprintService;
    private readonly ISprintRepository _sprintRepository;

    public TaskService(ITaskRepository taskRepository, IProjectService projectService, IWebHostEnvironment environment, ISprintService sprintService, ISprintRepository sprintRepository)
    {
        _taskRepository = taskRepository;
        _projectService = projectService;
        _environment = environment;
        _sprintService = sprintService;
        _sprintRepository = sprintRepository;
    }

    public async Task<ProjectTasksViewModel> GetProjectNamesAsync(UserViewModel user)
    {
        bool isAdmin = user.Role == "Admin";
        ProjectTasksViewModel model = new()
        {
            Projects = await _projectService.GetProjectNamesAsync(user.UserId, isAdmin),
        };
        return model;
    }

    public async Task<List<TaskViewModel>?> GetBacklogTasksByProjectIdAsync(int projectId)
    {
        List<ToDoList> tasks = await _taskRepository.GetBacklogTasksByProjectIdAsync(projectId);
        return tasks.Select(t => new TaskViewModel
        {
            TaskId = t.TaskId,
            ProjectId = projectId,
            Title = t.Title,
            Description = t.Description,
            Status = t.Status,
            AssignedTo = t.AssignedTo,
            Priority = t.Priority,
            Sprints = _sprintService.GetAllSprints(projectId).Result,
            TeamMembers = GetTeamMembersAsync(projectId).Result
        }).ToList();
    }

    public async Task<List<TaskViewModel>?> GetTasksBySprintIdAsync(int sprintId, int userId)
    {
        List<ToDoList> tasks = await _taskRepository.GetTasksBySprintIdAsync(sprintId);
        if (userId != 0)
        {
            tasks = tasks.Where(t => t.AssignedTo == userId).ToList();
        }
        return tasks.Select(t => new TaskViewModel
        {
            TaskId = t.TaskId,
            ProjectId = t.ProjectId,
            Title = t.Title,
            Description = t.Description,
            Status = t.Status,
            AssignedTo = t.AssignedTo,
            Priority = t.Priority,
            TeamMembers = GetTeamMembersAsync(t.ProjectId ?? 0).Result
        }).ToList();
    }

    public async Task<bool> UpdateTaskStatusAsync(int taskId, string newStatus)
    {
        ToDoList? task = await _taskRepository.GetTaskByIdAsync(taskId);
        if (task == null)
            return false;

        SprintViewModel? sprint = await _sprintService.GetSprintByIdAsync(task.SprintId ?? 0);
        if (sprint == null)
            return false;

        if (sprint.Status == "Not Started")
        {
            return false;
        }

        switch (newStatus)
        {
            case "To Do":
                task.CreatedOn = null;
                break;
            case "In Progress":
                if (task.Status == "To Do")
                {
                    task.CreatedOn = DateTime.UtcNow;
                }
                break;
            case "Done":
                if (task.Status != "In Progress")
                {
                    task.DueDate = DateTime.UtcNow;
                }
                break;
            default:
                break;
        }

        task.Status = newStatus;
        return await _taskRepository.UpdateTaskAsync(task);
    }

    public async Task<bool> UpdateTaskAsync(TaskViewModel model, int userId)
    {
        ToDoList? task = await _taskRepository.GetTaskByIdAsync(model.TaskId ?? 0);

        if (task == null)
            return false;

        if (model.SprintId != null && task.Status == "Not Started")
        {
            task.Status = "To Do";
        }
        else
        {
            task.Status = task.Status ?? "Not Started";
            task.CreatedOn = null;
            task.DueDate = null;
        }
        task.TaskId = model.TaskId ?? 0;
        task.Title = model.Title?.Trim();
        task.Description = model.Description;
        task.AssignedTo = model.AssignedTo;
        task.ProjectId = model.ProjectId;
        task.SprintId = model.SprintId ?? null;
        task.Priority = model.Priority;

        bool isUpdated = await _taskRepository.UpdateTaskAsync(task);

        if (model.TaskDetails != null && model.TaskDetails.Any() && isUpdated)
        {
            foreach (IFormFile? files in model.TaskDetails)
            {
                bool isImage = false;
                string[] permittedMimeTypes = ["image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp"];
                if (permittedMimeTypes.Contains(files.ContentType.ToLower()))
                {
                    isImage = true;
                }

                List<ToDoList> taskDetails = new();
                TaskAttachment attachment = new()
                {
                    TaskId = model.TaskId,
                    FileName = files.FileName,
                    IsImage = isImage,
                    UploadedOn = DateTime.UtcNow,
                    UploadedBy = userId,
                    IsDeleted = false
                };

                if (files != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/TaskDetails");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + files.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (FileStream? fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await files.CopyToAsync(fileStream);
                    }

                    attachment.FilePath = "/TaskDetails/" + uniqueFileName;
                }
                await _taskRepository.AddAttachmentAsync(attachment);
            }
        }

        if (model.TaskAttachments != null && model.TaskAttachments.Any())
        {
            foreach (TaskAttachmentViewModel attachment in model.TaskAttachments)
            {
                if (attachment.IsDeleted)
                {
                    TaskAttachment? taskAttachment = await _taskRepository.GetTaskAttachmentAsync(attachment.TaskAttachmentId);
                    if (taskAttachment == null)
                        return false;

                    taskAttachment.IsDeleted = true;
                    taskAttachment.DeletedBy = userId;
                    await _taskRepository.DeleteTaskAttachmentAsync(taskAttachment);
                }
            }
        }

        if (isUpdated)
        {

        }
        return true;
    }

    public async Task<List<UserViewModel>> GetTeamMembersAsync(int projectId)
    {
        List<ProjectUser> projectUsers = await _taskRepository.GetTeamMembersAsync(projectId);
        List<UserViewModel> teamMembers = [];
        foreach (ProjectUser teamMember in projectUsers)
        {
            teamMembers.Add(new UserViewModel
            {
                UserId = teamMember.UserId ?? 0,
                Name = teamMember.User?.Name
            });
        }
        return teamMembers;
    }

    public async Task<bool> AddTaskAsync(TaskViewModel model, UserViewModel user)
    {
        if (model.SprintId != null)
        {
            Sprint? sprint = await _sprintRepository.GetByIdAsync(model.SprintId.Value);
            if (sprint != null && sprint.Status == "Completed")
            {
                sprint.Status = "In Progress";
                await _sprintRepository.UpdateAsync(sprint);
            }
        }
        ToDoList task = new()
        {
            ProjectId = model.ProjectId,
            CreatedBy = user.UserId,
            AssignedTo = model.AssignedTo,
            Title = model.Title?.Trim(),
            Description = model.Description,
            Status = (model.SprintId != null) ? "To Do" : "Not Started",
            SprintId = model.SprintId ?? null,
            Priority = model.Priority,
            IsDeleted = false
        };

        ToDoList? addedTask = await _taskRepository.AddTaskAsync(task);
        if (model.TaskDetails != null && model.TaskDetails.Any())
        {
            foreach (IFormFile? files in model.TaskDetails)
            {
                bool isImage = false;
                string[] permittedMimeTypes = ["image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp"];
                if (permittedMimeTypes.Contains(files.ContentType.ToLower()))
                {
                    isImage = true;
                }

                List<ToDoList> taskDetails = new();
                TaskAttachment attachment = new()
                {
                    TaskId = addedTask.TaskId,
                    FileName = files.FileName,
                    IsImage = isImage,
                    UploadedOn = DateTime.UtcNow,
                    UploadedBy = user.UserId,
                    IsDeleted = false
                };
                if (files != null)
                {
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/TaskDetails");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + files.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (FileStream? fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await files.CopyToAsync(fileStream);
                    }

                    attachment.FilePath = "/TaskDetails/" + uniqueFileName;
                }
                await _taskRepository.AddAttachmentAsync(attachment);
            }
        }
        return true;
    }

    public async Task<bool> CheckTaskNameExists(string title, int taskId, int projectId)
    {
        ToDoList? task = await _taskRepository.GetTaskByTitleAsync(title.Trim().ToLower(), projectId);
        if (task != null)
        {
            if (task.TaskId == taskId)
            {
                return false;
            }
            return true;
        }
        return false;
    }

    public async Task<TaskViewModel?> GetTaskByIdAsync(int taskId)
    {
        ToDoList? task = await _taskRepository.GetTaskByIdAsync(taskId);
        if (task == null) return null;
        List<SprintViewModel> sprints = await _sprintService.GetAllSprints(task.ProjectId ?? 0);
        sprints = sprints.Where(s => s.Status != "Completed" || s.SprintId == task.SprintId).ToList();

        return new TaskViewModel
        {
            TaskId = task.TaskId,
            ProjectId = task.ProjectId,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            AssignedTo = task.AssignedTo,
            CreatedOn = task.CreatedOn,
            SprintId = task.SprintId,
            DueDate = task.DueDate,
            ProjectStartDate = task.Project?.CreatedOn ?? DateTime.Now,
            ProjectEndDate = task.Project?.EndDate ?? DateTime.Now,
            Priority = task.Priority,
            Sprints = sprints,
            TeamMembers = (await _taskRepository.GetTeamMembersAsync(task.ProjectId ?? 0))
                .Select(tm => new UserViewModel
                {
                    UserId = tm.UserId ?? 0,
                    Name = tm.User?.Name
                }).ToList(),
            TaskAttachments = _taskRepository.GetTaskAttachmentsByTaskIdAsync(task.TaskId).Result.Select(ta => new TaskAttachmentViewModel
            {
                TaskAttachmentId = ta.AttachmentId,
                TaskId = ta.TaskId ?? 0,
                FileName = ta.FileName,
                FilePath = ta.FilePath,
                IsImage = ta.IsImage ?? false,
                UploadedOn = ta.UploadedOn,
            }).ToList()
        };
    }

    public async Task<(FileStream? Stream, string? FileName)> DownloadFileAsync(int taskAttachmentId)
    {
        TaskAttachment? attachment = await _taskRepository.GetTaskAttachmentAsync(taskAttachmentId);

        if (attachment == null || string.IsNullOrWhiteSpace(attachment.FilePath))
            return (null, null);

        string? cleanedFilePath = attachment.FilePath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        string? fullPath = Path.Combine(_environment.WebRootPath, cleanedFilePath);

        if (!File.Exists(fullPath))
            return (null, null);

        FileStream? fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        return (fileStream, attachment.FileName);
    }

    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        ToDoList? task = await _taskRepository.GetTaskByIdAsync(taskId);
        if (task == null)
            return false;

        task.IsDeleted = true;
        return await _taskRepository.UpdateTaskAsync(task);
    }
}