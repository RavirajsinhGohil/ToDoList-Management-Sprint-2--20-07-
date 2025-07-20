using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoListManagement.Entity.Constants;
using ToDoListManagement.Entity.Helper;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Service.Helper;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Web.Controllers;

[Authorize]
public class EmployeeController : BaseController
{
    private readonly IEmployeeService _employeeService;
    public EmployeeController(IAuthService authService, IEmployeeService employeeService) : base(authService)
    {
        _employeeService = employeeService;
    }

    [CustomAuthorize([Constants.EmployeeModule], Constants.CanView)]
    public IActionResult Index()
    {
        if (SessionUser == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        return View();
    }

    [CustomAuthorize([Constants.EmployeeModule], Constants.CanView)]
    [HttpGet]
    public async Task<JsonResult> GetEmployees(int draw, int start, int length, string searchValue, string sortColumn, string sortDirection)
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

        Pagination<EmployeeViewModel>? data = await _employeeService.GetPaginatedEmployeesAsync(pagination, SessionUser);

        return Json(new
        {
            draw,
            recordsTotal = data.TotalRecords,
            recordsFiltered = data.TotalRecords,
            data = data.Items
        });
    }

    [CustomAuthorize([Constants.EmployeeModule], Constants.CanAddEdit)]
    [HttpGet]
    public async Task<IActionResult> AddEmployee()
    {
        if (SessionUser == null)
        {
            return RedirectToAction("Login", "Auth");
        }
        EmployeeViewModel employee = new()
        {
            Roles = await _employeeService.GetRolesForEmployeeAsync()
        };
        return View(employee);
    }

    [CustomAuthorize([Constants.EmployeeModule], Constants.CanAddEdit)]
    [HttpPost]
    public async Task<IActionResult> AddEmployee(EmployeeViewModel employee)
    {
        if (ModelState.IsValid)
        {
            bool isAdded = await _employeeService.AddEmployeeAsync(employee);
            if(isAdded)
            {
                TempData["SuccessMessage"] = Constants.EmployeeAddedMessage;
            }
            else
            {
                TempData["ErrorMessage"] = Constants.EmployeeAddFailedMessage;
            }
            
            return RedirectToAction("Index", "Employee");
        }
        return View(employee);
    }

    [CustomAuthorize([Constants.EmployeeModule], Constants.CanAddEdit)]
    [HttpGet]
    public async Task<IActionResult> GetEmployeeById(int employeeId)
    {
        if (SessionUser == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        EditEmployeeViewModel employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
        if (employee == null)
        {
            return RedirectToAction("ErrorPage", "ShowError", new { statusCode = 404 });
        }

        return View("EditEmployee", employee);
    }

    [CustomAuthorize([Constants.EmployeeModule], Constants.CanAddEdit)]
    [HttpPost]
    public async Task<IActionResult> UpdateEmployee(EditEmployeeViewModel employee)
    {
        if (ModelState.IsValid)
        {
            bool isUpdated = await _employeeService.UpdateEmployeeAsync(employee);
            if (isUpdated)
            {
                TempData["SuccessMessage"] = Constants.EmployeeUpdatedMessage;
            }
            else
            {
                TempData["ErrorMessage"] = Constants.EmployeeUpdateFailedMessage;
            }
            return RedirectToAction("Index", "Employee");
        }
        return View(employee);
    }

    [CustomAuthorize([Constants.EmployeeModule], Constants.CanDelete)]
    [HttpGet]
    public async Task<IActionResult> DeleteEmployee(int employeeId)
    {
        if (SessionUser == null)
        {
            return RedirectToAction("Login", "Auth");
        }
        bool isDeleted = await _employeeService.DeleteEmployeeAsync(employeeId);
        if (isDeleted)
        {
            TempData["SuccessMessage"] = Constants.EmployeeDeletedMessage;
        }
        else
        {
            TempData["ErrorMessage"] = Constants.EmployeeDeleteFailedMessage;
        }
        return RedirectToAction("Index", "Employee");
    }
}