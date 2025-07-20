using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoListManagement.Entity.Constants;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Service.Helper;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Web.Controllers;

[Authorize]
public class TasksController : BaseController
{
    private readonly ITaskService _taskService;
    private readonly IProjectService _projectService;
    private readonly ISprintService _sprintService;

    public TasksController(IAuthService authService, ITaskService taskService, IProjectService projectService, ISprintService sprintService)
        : base(authService)
    {
        _taskService = taskService;
        _projectService = projectService;
        _sprintService = sprintService;
    }

    [CustomAuthorize([Constants.TaskBoardModule], Constants.CanView)]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (SessionUser != null)
        {
            ProjectTasksViewModel model = await _taskService.GetProjectNamesAsync(SessionUser);
            return View(model);
        }
        return RedirectToAction("Login", "Auth");
    }

    [CustomAuthorize([Constants.TaskBoardModule], Constants.CanView)]
    [HttpGet]
    public async Task<IActionResult> GetProjectsJson()
    {
        if (SessionUser == null)
        {
            return RedirectToAction("Login", "Auth");
        }
        ProjectTasksViewModel model = await _taskService.GetProjectNamesAsync(SessionUser);
        return Json(model.Projects);
    }

    [CustomAuthorize([Constants.TaskBoardModule], Constants.CanView)]
    [HttpGet]
    public async Task<IActionResult> GetTasksBySprintId(int sprintId, int userId)
    {
        List<TaskViewModel>? tasks = await _taskService.GetTasksBySprintIdAsync(sprintId, userId);
        return PartialView("_TasksList", tasks ?? []);
    }

    [CustomAuthorize([Constants.TaskBoardModule], Constants.CanAddEdit)]
    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int taskId, string newStatus)
    {
        bool result = await _taskService.UpdateTaskStatusAsync(taskId, newStatus);
        if (result)
        {
            return Json(new { success = true, message = Constants.TaskStausChangeMessage });
        }
        return Json(new { success = false, message = Constants.TaskStausChangeFailedMessage });
    }

    [CustomAuthorize([Constants.TaskBoardModule], Constants.CanView)]
    [HttpGet]
    public async Task<IActionResult> GetSprintsJson(int projectId)
    {
        List<SprintViewModel> sprints = await _sprintService.GetAllSprints(projectId);
        return Json(sprints);
    }

    [CustomAuthorize([Constants.TaskBoardModule], Constants.CanView)]
    [HttpGet]
    public async Task<IActionResult> GetTeamMembersJson(int projectId)
    {
        List<UserViewModel> teamMembers = await _taskService.GetTeamMembersAsync(projectId);
        return Json(teamMembers);
    }

    [CustomAuthorize([Constants.TaskBoardModule], Constants.CanAddEdit)]
    [HttpGet]
    public async Task<IActionResult> AddTask(int projectId)
    {
        ProjectViewModel? project = await _projectService.GetProjectByIdAsync(projectId);
        List<SprintViewModel> sprints = await _sprintService.GetAllSprints(projectId);
        sprints = sprints.Where(s => s.Status != "Completed").ToList();

        TaskViewModel task = new()
        {
            ProjectId = projectId,
            ProjectStartDate = project?.StartDate ?? DateTime.UtcNow,
            ProjectEndDate = project?.EndDate ?? DateTime.UtcNow,
            TeamMembers = SessionUser != null ? await _taskService.GetTeamMembersAsync(projectId) : [],
            Sprints = sprints
        };
        return PartialView("_AddTaskModal", task);
    }

    [CustomAuthorize([Constants.TaskBoardModule], Constants.CanAddEdit)]
    public async Task<IActionResult> CheckTaskTitleExists(string title, int taskId = 0, int projectId = 0)
    {
        await Task.Delay(1000);
        bool exists = await _taskService.CheckTaskNameExists(title.Trim(), taskId, projectId);
        return Json(!exists);
    }

    [CustomAuthorize([Constants.TaskBoardModule], Constants.CanAddEdit)]
    [HttpPost]
    public async Task<IActionResult> AddTask(TaskViewModel model)
    {
        if (SessionUser?.UserId == null)
        {
            TempData["ErrorMessage"] = Constants.InvalidSessionMessage;
            return Json(new { success = false, message = Constants.InvalidSessionMessage });
        }

        bool isAdded = await _taskService.AddTaskAsync(model, SessionUser);
        return Json(new { success = isAdded, message = isAdded ? Constants.TaskAddedMessage : Constants.TaskAddFailedMessage });
    }

    [CustomAuthorize([Constants.TaskBoardModule], Constants.CanView)]
    [HttpGet]
    public async Task<IActionResult> GetTaskById(int taskId)
    {
        TaskViewModel? task = await _taskService.GetTaskByIdAsync(taskId);
        return PartialView("_UpdateTaskModal", task);
    }

    [HttpGet]
    public async Task<IActionResult> DownloadFile(int taskAttachmentId)
    {
        (FileStream? fileStream, string? fileName) = await _taskService.DownloadFileAsync(taskAttachmentId);

        if (fileStream != null && !string.IsNullOrEmpty(fileName))
        {
            return File(fileStream, "application/octet-stream", fileName);
        }

        return NotFound();
    }

    [CustomAuthorize([Constants.TaskBoardModule], Constants.CanAddEdit)]
    [HttpPost]
    public async Task<IActionResult> UpdateTask(TaskViewModel model)
    {
        if (SessionUser?.UserId == null)
        {
            TempData["ErrorMessage"] = Constants.InvalidSessionMessage;
            return Json(new { success = false, message = Constants.InvalidSessionMessage });
        }

        bool isUpdated = await _taskService.UpdateTaskAsync(model, SessionUser.UserId);
        return Json(new { success = isUpdated, message = isUpdated ? Constants.TaskUpdatedMessage : Constants.TaskUpdateFailedMessage });
    }

    [CustomAuthorize([Constants.TaskBoardModule], Constants.CanDelete)]
    [HttpGet]
    public async Task<IActionResult> DeleteTask(int taskId)
    {
        bool isDeleted = await _taskService.DeleteTaskAsync(taskId);
        return Json(new { success = isDeleted, message = isDeleted ? Constants.TaskDeletedMessage : Constants.TaskDeleteFailedMessage });
    }

    [CustomAuthorize([Constants.TaskBoardModule], Constants.CanAddEdit)]
    [HttpGet]
    public async Task<IActionResult> AddSprint(int projectId)
    {
        ProjectViewModel? project = await _projectService.GetProjectByIdAsync(projectId);

        SprintViewModel sprint = new()
        {
            ProjectId = projectId,
            ProjectStartDate = project?.StartDate ?? DateTime.UtcNow,
            ProjectEndDate = project?.EndDate ?? DateTime.UtcNow,
            ScrumMasterId = project?.AssignedToScrumMaster
        };
        return PartialView("_AddSprintModal", sprint);
    }

    [CustomAuthorize([Constants.TaskBoardModule], Constants.CanAddEdit)]
    public async Task<IActionResult> CheckSprintNameExists(string name, int projectId = 0)
    {
        await Task.Delay(1000);
        bool exists = await _sprintService.CheckSprintNameExistsAsync(name.Trim(), projectId);
        return Json(!exists);
    }

    [CustomAuthorize([Constants.TaskBoardModule], Constants.CanAddEdit)]
    [HttpPost]
    public async Task<IActionResult> AddSprint(SprintViewModel model)
    {
        bool isAdded = await _sprintService.AddSprintAsync(model);
        return Json(new { success = isAdded, message = isAdded ? Constants.SprintAddedMessage : Constants.SprintAddFailedMessage });
    }

    [CustomAuthorize([Constants.TaskBoardModule], Constants.CanView)]
    [HttpGet]
    public async Task<IActionResult> GetBacklogTasks(int projectId)
    {
        List<TaskViewModel>? tasks = await _taskService.GetBacklogTasksByProjectIdAsync(projectId);
        return PartialView("_BacklogTasks", tasks);
    }

    [CustomAuthorize([Constants.TaskBoardModule], Constants.CanView)]
    [HttpGet]
    public async Task<IActionResult> GetSprintById(int sprintId)
    {
        SprintViewModel? sprint = await _sprintService.GetSprintByIdAsync(sprintId);
        return Json(sprint);
    }

    [CustomAuthorize([Constants.TaskBoardModule], Constants.CanAddEdit)]
    [HttpPost]
    public async Task<IActionResult> StartSprint(int sprintId)
    {
        bool isStarted = await _sprintService.StartSprintAsync(sprintId);
        return Json(new { success = isStarted, message = isStarted ? Constants.SprintStartedMessage : Constants.SprintStartFailedMessage });
    }

    [CustomAuthorize([Constants.TaskBoardModule], Constants.CanAddEdit)]
    [HttpPost]
    public async Task<IActionResult> CompleteSprint(int sprintId)
    {
        bool isCompleted = await _sprintService.CompleteSprintAsync(sprintId);
        return Json(new { success = isCompleted, message = isCompleted ? Constants.SprintCompletedMessage : Constants.SprintCompleteFailedMessage });
    }
}