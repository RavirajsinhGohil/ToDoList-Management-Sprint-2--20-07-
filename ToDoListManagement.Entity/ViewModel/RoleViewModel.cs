using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ToDoListManagement.Entity.ViewModel;

public class RoleViewModel
{
    public int RoleId { get; set; }

    [Required(ErrorMessage = Constants.Constants.RoleNameRequiredError)]
    [MaxLength(50)]
    // [Remote("CheckRoleExists", "RoleAndPermission", ErrorMessage = Constants.Constants.RoleNameAlreadyExistsError)]
    [Remote(action: "CheckRoleExists", controller: "RoleAndPermission", AdditionalFields = nameof(RoleId), ErrorMessage = Constants.Constants.RoleNameAlreadyExistsError)]
    public string? RoleName { get; set; }

    public bool IsDeleted { get; set; }
}