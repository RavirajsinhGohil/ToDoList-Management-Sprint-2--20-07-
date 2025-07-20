using Microsoft.AspNetCore.Mvc;
using ToDoListManagement.Entity.Constants;
using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Service.Helper;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Web.Controllers;

public class ProjectController : BaseController
{
    private readonly IProjectService _projectService;
    public ProjectController(IAuthService authService, IProjectService projectService) : base(authService)
    {
        _projectService = projectService;
    }

    [CustomAuthorize([Constants.ProjectModule], Constants.CanView)]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (ViewBag.SessionUser == null || string.IsNullOrEmpty(ViewBag.SessionUser.Email))
        {
            return RedirectToAction("Login", "Auth");
        }
        ProjectListViewModel model = new ();
        model.ProjectManagers = await _projectService.GetProjectManagersAsync();
        return View(model);
    }

    [CustomAuthorize([Constants.ProjectModule], Constants.CanView)]
    [HttpGet]
    public async Task<JsonResult> GetProjects(int draw, int start, int length, string searchValue, string sortColumn, string sortDirection)
    {
        int pageNumber = start / length + 1;
        int pageSize = length;

        Pagination<ProjectViewModel>? pagination = new()
        {
            SearchKeyword = searchValue,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            SortColumn = sortColumn,
            SortDirection = sortDirection
        };

        if (SessionUser == null)
        {
            return Json(new
            {
                draw,
                recordsTotal = 0,
                recordsFiltered = 0,
                data = Array.Empty<ProjectViewModel>()
            });
        }

        Pagination<ProjectViewModel>? data = await _projectService.GetPaginatedProjectsAsync(pagination, SessionUser);

        return Json(new
        {
            draw,
            recordsTotal = data.TotalRecords,
            recordsFiltered = data.TotalRecords,
            data = data.Items
        });
    }

    [CustomAuthorize([Constants.ProjectModule], Constants.CanAddEdit)]
    [HttpGet]
    public async Task<IActionResult> AddProject()
    {
        List<UserViewModel> projectManagers = await _projectService.GetProjectManagersAsync();
        List<UserViewModel> scrumMasters = await _projectService.GetScrumMastersAsync();
        ProjectViewModel model = new()
        {
            ProjectManagers = projectManagers,
            ScrumMasters = scrumMasters
        };

        return PartialView("_AddProjectModal", model);
    }

    [CustomAuthorize([Constants.ProjectModule], Constants.CanView)]
    public async Task<IActionResult> CheckProjectNameExists(string projectName, int projectId = 0)
    {
        await Task.Delay(1000);
        bool exists = await _projectService.CheckProjectNameExistsAsync(projectName.Trim(), projectId);
        return Json(!exists);
    }

    [CustomAuthorize([Constants.ProjectModule], Constants.CanAddEdit)]
    [HttpPost]
    public async Task<IActionResult> AddProject(ProjectViewModel model)
    {
        if (SessionUser == null)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        bool isAdded = await _projectService.AddProject(model, SessionUser.UserId);
        if (isAdded)
        {
            TempData["SuccessMessage"] = Constants.ProjectAddedMessage;
        }
        else
        {
            TempData["ErrorMessage"] = Constants.ProjectAddFailedMessage;
        }
        return RedirectToAction("Index", "Project");
    }

    [CustomAuthorize([Constants.ProjectModule], Constants.CanAddEdit)]
    [HttpGet]
    public async Task<IActionResult> GetProjectById(int projectId)
    {
        ProjectViewModel? model = await _projectService.GetProjectByIdAsync(projectId);
        if (model == null)
        {
            return Json(new { success = false, message = Constants.ProjectNotFoundMessage });
        }
    
        return PartialView("_UpdateProjectModal", model);
    }

    [CustomAuthorize([Constants.ProjectModule], Constants.CanAddEdit)]
    [HttpPost]
    public async Task<IActionResult> UpdateProject(ProjectViewModel model)
    {
        bool isUpdated = await _projectService.UpdateProjectAsync(model);
        if (isUpdated)
        {
            TempData["SuccessMessage"] = Constants.ProjectUpdatedMessage;
        }
        else
        {
            TempData["ErrorMessage"] = Constants.ProjectUpdateFailedMessage;
        }
        return RedirectToAction("Index", "Project");
    }

    [CustomAuthorize([Constants.ProjectModule], Constants.CanAddEdit)]
    [HttpGet]
    public async Task<IActionResult> GetAssignedMembers(int projectId, int draw, int start, int length, string searchValue, string sortColumn, string sortDirection)
    {
        int pageNumber = start / length + 1;
        int pageSize = length;

        Pagination<MemberViewModel>? pagination = new()
        {
            SearchKeyword = searchValue,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            SortColumn = sortColumn,
            SortDirection = sortDirection
        };
        
        Pagination<MemberViewModel> data = await _projectService.GetAssignedMembersAsync(pagination, projectId);

        return Json(new
        {
            draw,
            recordsTotal = data.TotalRecords,
            recordsFiltered = data.TotalRecords,
            data = data.Items
        });
    }

    [CustomAuthorize([Constants.ProjectModule], Constants.CanAddEdit)]
    [HttpPost]
    public async Task<IActionResult> AssignMembers([FromBody] AssignMembersViewModel request)
    {
        bool isAssigned =  await _projectService.AssignMembersAsync(request.ProjectId, request.UserIds);
        if(isAssigned)
        {
            return Json(new { success = true, message = Constants.MembersAssignedMessage });
        }
        else
        {
            return Json(new { success = false, message = Constants.MembersAssignedFailedMessage });
        }
    }

    [CustomAuthorize([Constants.ProjectModule], Constants.CanDelete)]
    [HttpGet]
    public async Task<IActionResult> DeleteProject(int projectId)
    {
        if (SessionUser == null)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        bool isDeleted = await _projectService.DeleteProjectAsync(projectId, SessionUser.UserId);
        if (isDeleted)
        {
            TempData["SuccessMessage"] = Constants.ProjectDeletedMessage;
        }
        else
        {
            TempData["ErrorMessage"] = Constants.ProjectDeleteFailedMessage;
        }
        
        return RedirectToAction("Index", "Project");
    }
}