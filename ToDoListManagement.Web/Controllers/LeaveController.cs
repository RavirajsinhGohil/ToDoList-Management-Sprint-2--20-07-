using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoListManagement.Entity.Constants;
using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Service.Helper;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Web.Controllers;

[Authorize]
public class LeaveController : BaseController
{
    private readonly ILeaveService _leaveService;

    public LeaveController(IAuthService authService, ILeaveService leaveService)
        : base(authService)
    {
        _leaveService = leaveService;
    }

    #region Self Leave

    [CustomAuthorize([Constants.SelfLeaveModule], Constants.CanView)]
    public IActionResult SelfLeave()
    {
        ViewData["IsTeamLeave"] = false;
        return View();
    }

    [CustomAuthorize([Constants.SelfLeaveModule], Constants.CanView)]
    public async Task<JsonResult> GetSelfLeaves(int draw, int start, int length, string searchValue, string sortColumn, string sortDirection, string? statusFilter, string? startDateFilter, string? endDateFilter)
    {
        int pageNumber = start / length + 1;
        int pageSize = length;

        DateOnly? startDateDateOnly = null;
        DateOnly? endDateDateOnly = null;

        if (startDateFilter != null)
        {
            startDateDateOnly = DateOnly.Parse(startDateFilter);
        }

        if (endDateFilter != null)
        {
            endDateDateOnly = DateOnly.Parse(endDateFilter);
        }

        Pagination<LeaveViewModel>? pagination = new()
        {
            SearchKeyword = searchValue,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            SortColumn = sortColumn,
            SortDirection = sortDirection,
            StatusFilter = statusFilter,
            StartDate = startDateDateOnly,
            EndDate = endDateDateOnly
        };
        if (SessionUser == null)
        {
            return Json(new
            {
                draw,
                recordsTotal = 0,
                recordsFiltered = 0,
                data = Array.Empty<LeaveViewModel>()
            });
        }
        Pagination<LeaveViewModel>? data = await _leaveService.GetPaginatedLeavesAsync(pagination, SessionUser.UserId);

        return Json(new
        {
            draw,
            recordsTotal = data.TotalRecords,
            recordsFiltered = data.TotalRecords,
            data = data.Items
        });
    }

    [HttpGet]
    [CustomAuthorize([Constants.SelfLeaveModule], Constants.CanAddEdit)]
    public IActionResult AddLeave()
    {
        if (SessionUser == null)
        {
            return RedirectToAction("Login", "Auth");
        }
        LeaveViewModel leaveViewModel = new()
        {
            RequestedUserId = SessionUser.UserId,
            RequestedName = SessionUser.Name,
            ApprovalUserId = SessionUser.UserId,
            ApprovalName = SessionUser.Name,
        };
        return leaveViewModel == null
            ? Json(new { success = false, message = Constants.LeaveNotFound })
            : Json(new { success = true, data = leaveViewModel });
    }

    [HttpPost]
    [CustomAuthorize([Constants.SelfLeaveModule], Constants.CanAddEdit)]
    public async Task<IActionResult> AddLeave(LeaveViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (SessionUser == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            bool isAdded = await _leaveService.AddLeaveAsync(model, SessionUser);
            return isAdded
                ? Json(new { success = true, message = Constants.LeaveAddedMessage })
                : Json(new { success = false, message = Constants.LeaveAddFailedMessage });
        }
        else
        {
            return Json(new { success = false, message = Constants.LeaveAddFailedMessage });
        }
    }

    [HttpGet]
    [CustomAuthorize([Constants.SelfLeaveModule, Constants.TeamLeaveModule], Constants.CanAddEdit)]
    public async Task<IActionResult> GetLeaveById(int leaveId)
    {
        if (SessionUser == null)
        {
            return RedirectToAction("Login", "Auth");
        }
        LeaveViewModel? leave = await _leaveService.GetLeaveByIdAsync(leaveId);
        if (leave == null)
        {
            return Json(new { success = false, message = Constants.LeaveNotFound });
        }
        return Json(new { success = true, data = leave });
    }

    [HttpPost]
    [CustomAuthorize([Constants.SelfLeaveModule, Constants.TeamLeaveModule], Constants.CanAddEdit)]
    public async Task<IActionResult> UpdateLeave(LeaveViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (SessionUser == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            bool isUpdated = await _leaveService.UpdateLeaveAsync(model);
            return isUpdated
                ? Json(new { success = true, message = Constants.LeaveUpdatedMessage })
                : Json(new { success = false, message = Constants.LeaveUpdateFailedMessage });
        }
        else
        {
            return Json(new { success = false, message = Constants.LeaveUpdateFailedMessage });
        }
    }

    [HttpGet]
    [CustomAuthorize([Constants.SelfLeaveModule, Constants.TeamLeaveModule], Constants.CanDelete)]
    public async Task<IActionResult> DeleteLeave(int leaveId)
    {
        if (SessionUser == null)
        {
            return RedirectToAction("Login", "Auth");
        }
        bool isDeleted = await _leaveService.DeleteLeaveAsync(leaveId);
        return isDeleted ?
            Json(new { success = true, message = Constants.LeaveDeletedMessage }) :
            Json(new { success = false, message = Constants.LeaveDeleteFailedMessage });
    }

    #endregion

    #region Team Leave

    [CustomAuthorize([Constants.TeamLeaveModule], Constants.CanView)]
    public IActionResult TeamLeave()
    {
        ViewData["IsTeamLeave"] = true;
        return View();
    }

    [CustomAuthorize([Constants.TeamLeaveModule], Constants.CanView)]
    public async Task<IActionResult> GetTeamLeaves(int draw, int start, int length, string searchValue, string sortColumn, string sortDirection, string? statusFilter, string? startDateFilter, string? endDateFilter)
    {
        int pageNumber = start / length + 1;
        int pageSize = length;

        DateOnly? startDateDateOnly = null;
        DateOnly? endDateDateOnly = null;

        if (startDateFilter != null)
        {
            startDateDateOnly = DateOnly.Parse(startDateFilter);
        }

        if (endDateFilter != null)
        {
            endDateDateOnly = DateOnly.Parse(endDateFilter);
        }

        Pagination<LeaveViewModel>? pagination = new()
        {
            SearchKeyword = searchValue,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            SortColumn = sortColumn,
            SortDirection = sortDirection,
            StatusFilter = statusFilter,
            StartDate = startDateDateOnly,
            EndDate = endDateDateOnly
        };
        Pagination<LeaveViewModel>? data = await _leaveService.GetPaginatedLeavesAsync(pagination, null);

        return Json(new
        {
            draw,
            recordsTotal = data.TotalRecords,
            recordsFiltered = data.TotalRecords,
            data = data.Items
        });
    }

    #endregion

}