using Microsoft.AspNetCore.Mvc;
using ToDoListManagement.Entity.Constants;
using ToDoListManagement.Entity.ViewModel;
using ToDoListManagement.Service.Helper;
using ToDoListManagement.Service.Interfaces;

namespace ToDoListManagement.Web.Controllers;

public class RoleAndPermissionController : BaseController
{
    private readonly IRoleService _roleService;
    private readonly IPermissionService _permissionService;

    public RoleAndPermissionController(IRoleService roleService, IPermissionService permissionService, IAuthService authService) : base(authService)
    {
        _roleService = roleService;
        _permissionService = permissionService;
    }

    [CustomAuthorize([Constants.RolePermissionModule], Constants.CanView)]
    public async Task<IActionResult> Roles()
    {
        List<RoleViewModel> roles = await _roleService.GetAllRoles();
        return View(roles);
    }

    [CustomAuthorize([Constants.RolePermissionModule], Constants.CanAddEdit)]
    [HttpPost]
    public async Task<IActionResult> AddRole(RoleViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (SessionUser?.UserId == null)
            {
                TempData["ErrorMessage"] = Constants.InvalidSessionMessage;
                return RedirectToAction("Roles", "RoleAndPermission");
            }
            RoleViewModel role = await _roleService.AddRoleAsync(model, SessionUser.UserId);
            if (role != null)
            {
                TempData["SuccessMessage"] = Constants.RoleAddedMessage;
            }
            else
            {
                TempData["ErrorMessage"] = Constants.RoleAddFailedMessage;
                return RedirectToAction("Roles", "RoleAndPermission");
            }
            return RedirectToAction("Permissions", "RoleAndPermission", new { roleId = role.RoleId });
        }
        else
        {
            return PartialView("_AddRoleModal", model);
        }
    }

    [CustomAuthorize([Constants.RolePermissionModule], Constants.CanAddEdit)]
    public async Task<IActionResult> CheckRoleExists(string roleName, int roleId = 0)
    {
        await Task.Delay(1000);
        bool exists = await _roleService.CheckRoleNameExistsAsync(roleName, roleId);
        return Json(!exists);
    }

    [CustomAuthorize([Constants.RolePermissionModule], Constants.CanAddEdit)]
    [HttpGet]
    public async Task<IActionResult> GetRoleById(int roleId)
    {
        RoleViewModel? model = await _roleService.GetRoleByIdAsync(roleId);
        if (model == null)
        {
            return Json(new { success = false, message = Constants.RoleNotFoundMessage });
        }

        return PartialView("_UpdateRoleModal", model);
    }

    [CustomAuthorize([Constants.RolePermissionModule], Constants.CanAddEdit)]
    [HttpPost]
    public async Task<IActionResult> UpdateRole(RoleViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (SessionUser?.UserId == null)
            {
                TempData["ErrorMessage"] = Constants.InvalidSessionMessage;
                return RedirectToAction("Roles", "RoleAndPermission");
            }

            bool isUpdated = await _roleService.UpdateRoleAsync(model, SessionUser.UserId);
            if (isUpdated)
            {
                TempData["SuccessMessage"] = Constants.RoleUpdatedMessage;
            }
            else
            {
                TempData["ErrorMessage"] = Constants.RoleUpdateFailedMessage;
            }
            return RedirectToAction("Roles", "RoleAndPermission");
        }
        else
        {
            return PartialView("_UpdateRoleModal", model);
        }
    }

    [CustomAuthorize([Constants.RolePermissionModule], Constants.CanDelete)]
    [HttpGet]
    public async Task<IActionResult> DeleteRole(int roleId)
    {
        if (SessionUser?.UserId == null)
        {
            TempData["ErrorMessage"] = Constants.InvalidSessionMessage;
            return RedirectToAction("Roles", "RoleAndPermission");
        }
        bool usersRoleExists = await _roleService.CheckUsersRoleExists(roleId);
        if (usersRoleExists)
        {
            TempData["ErrorMessage"] = Constants.RoleDeleteAssignedUsersFailedMessage;
        }
        else
        {
            bool isDeleted = await _roleService.DeleteRoleAsync(roleId, SessionUser.UserId);
            if (isDeleted)
            {
                TempData["SuccessMessage"] = Constants.RoleDeletedMessage;
            }
            else
            {
                TempData["ErrorMessage"] = Constants.RoleDeleteFailedMessage;
            }
        }

        return RedirectToAction("Roles", "RoleAndPermission");
    }

    [CustomAuthorize([Constants.RolePermissionModule], Constants.CanView)]
    [HttpGet]
    public async Task<IActionResult> Permissions(int roleId)
    {
        List<PermissionViewModel> model = await _permissionService.GetPermissionsByRoleAsync(roleId);
        if(model == null)
        {
            return NotFound();
        }
        return View(model);
    }

    [CustomAuthorize([Constants.RolePermissionModule], Constants.CanAddEdit)]
    [HttpPost]
    public async Task<IActionResult> UpdatePermissions(List<PermissionViewModel> updatedPermissions)
    {
        if (SessionUser?.UserId == null)
        {
            TempData["ErrorMessage"] = Constants.InvalidSessionMessage;
            return RedirectToAction("Roles", "RoleAndPermission");
        }
        bool isUpdated = await _permissionService.UpdatePermissionsAsync(updatedPermissions, SessionUser.UserId);
        if (isUpdated)
        {
            TempData["SuccessMessage"] = Constants.PermissionUpdatedMessage;
        }
        else
        {
            TempData["ErrorMessage"] = Constants.PermissionUpdateFailedMessage;
        }
        return RedirectToAction("Roles", "RoleAndPermission");
    }
}