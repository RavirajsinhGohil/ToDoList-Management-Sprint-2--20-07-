using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoListManagement.Entity.Constants;
using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Service.Helper;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Web.Controllers;

[Authorize]
public class TeamController : BaseController
{
    private readonly ITeamService _teamService;
    public TeamController(IAuthService authService, ITeamService teamService) : base(authService)
    {
        _teamService = teamService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetAssignedMembers(int draw, int start, int length, string searchValue, string sortColumn, string sortDirection)
    {
        int pageNumber = start / length + 1;
        int pageSize = length;

        Pagination<EmployeeViewModel>? pagination = new()
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
                data = Array.Empty<EmployeeViewModel>()
            });
        }
        Pagination<EmployeeViewModel>? data = await _teamService.GetMembersAsync(pagination, SessionUser.UserId);
        return Json(new
        {
            draw,
            recordsTotal = data.TotalRecords,
            recordsFiltered = data.TotalRecords,
            data = data.Items
        });
    }

    public async Task<IActionResult> GetNotAssignedMembers()
    {
        List<EmployeeViewModel>? data = await _teamService.GetNotAssignedMembersAsync();
        return Json(data);
    }

    [CustomAuthorize([Constants.ManageTeamModule], Constants.CanAddEdit)]
    [HttpGet]
    public async Task<IActionResult> GetTeamMembersJson()
    {
        if (SessionUser == null)
        {
            return RedirectToAction("Login", "Auth");
        }
        List<UserViewModel> model = await _teamService.GetAllTeamMemberNamesAsync(SessionUser.UserId);
        return Json(model);
    }

    [CustomAuthorize([Constants.ManageTeamModule], Constants.CanAddEdit)]
    [HttpPost]
    public async Task<IActionResult> AddTeamMembers(TeamMembersViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (SessionUser == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            bool isAdded = await _teamService.AddTeamMembersAsync(model, SessionUser.UserId);
            return isAdded
                ? Json(new { success = true, message = Constants.TeamMembersAddedMessage })
                : Json(new { success = false, message = Constants.TeamMembersAddFailedMessage });
        }
        else
        {
            return Json(new { success = false, message = Constants.TeamMembersAddFailedMessage });
        }
    }

    [CustomAuthorize([Constants.ManageTeamModule], Constants.CanAddEdit)]
    [HttpGet]
    public async Task<IActionResult> GetTeamMemberById(int teamMemberId)
    {
        if (SessionUser == null)
        {
            return RedirectToAction("Login", "Auth");
        }
        TeamViewModel? model = await _teamService.GetTeamMemberByIdAsync(teamMemberId, SessionUser);
        return model != null
                ? Json(new { success = true, data = model })
                : Json(new { success = false, message = Constants.TeamMemberGetFailedMessage });
    }

    [CustomAuthorize([Constants.ManageTeamModule], Constants.CanAddEdit)]
    [HttpPost]
    public async Task<IActionResult> ChangeTeamMember(TeamViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (SessionUser == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            bool isAdded = await _teamService.ChangeTeamMemberAsync(model);
            return isAdded
                ? Json(new { success = true, message = Constants.TeamMembersUpdatedMessage })
                : Json(new { success = false, message = Constants.TeamMembersUpdatedFailedMessage });
        }
        else
        {
            return Json(new { success = false, message = Constants.TeamMembersUpdatedFailedMessage });
        }
    }

    [CustomAuthorize([Constants.ManageTeamModule], Constants.CanDelete)]
    [HttpGet]
    public async Task<IActionResult> RemoveTeamMember(int teamMemberId)
    {
        if (SessionUser == null)
        {
            return RedirectToAction("Login", "Auth");
        }
        bool isAdded = await _teamService.RemoveTeamMemberAsync(teamMemberId);
        return isAdded
            ? Json(new { success = true, message = Constants.TeamMembersRemovedMessage })
            : Json(new { success = false, message = Constants.TeamMembersRemoveFailedMessage });
    }

    [CustomAuthorize([Constants.ManageTeamModule], Constants.CanView)]
    public IActionResult TeamHierarchy()
    {
        return View();
    }
    
    [CustomAuthorize([Constants.ManageTeamModule], Constants.CanView)]
    [HttpGet]
    public async Task<IActionResult> GetTeamsList()
    {
        if (SessionUser == null)
        {
            return RedirectToAction("Login", "Auth");
        }
        List<TeamViewModel> data = await _teamService.GetMembersByTeamManagerIdAsync(SessionUser.UserId);
        return Json(data);
    }
}