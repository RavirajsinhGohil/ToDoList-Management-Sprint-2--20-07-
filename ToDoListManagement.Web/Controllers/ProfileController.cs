using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoListManagement.Entity.Constants;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Web.Controllers;

[Authorize]
public class ProfileController : BaseController
{
    private readonly IProfileService _profileService;

    public ProfileController(IAuthService authService, IProfileService profileService) : base(authService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ProfileViewModel? user = await _profileService.GetUserProfileAsync(SessionUser?.Email);
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
    {
        if (ModelState.IsValid)
        {
            bool isUpdated = await _profileService.UpdateUserProfileAsync(model);
            if (isUpdated)
            {
                TempData["SuccessMessage"] = Constants.ProfileUpdatedMessage;

            }
            else
            {
                TempData["ErrorMessage"] = Constants.ProfileUpdateFailedMessage;
            }
            return RedirectToAction("Index", "Dashboard");
        }
        return View("Index", model);
    }
}