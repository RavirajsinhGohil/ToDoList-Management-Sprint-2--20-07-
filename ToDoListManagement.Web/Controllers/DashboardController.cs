using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Web.Controllers;

[Authorize]
public class DashboardController : BaseController
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IAuthService authService, IDashboardService dashboardService)
        : base(authService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        if (SessionUser == null)
        {
            return RedirectToAction("Login", "Auth");
        }
        DashboardViewModel model = await _dashboardService.GetDashboardData(SessionUser);
        return View(model);
    }
}