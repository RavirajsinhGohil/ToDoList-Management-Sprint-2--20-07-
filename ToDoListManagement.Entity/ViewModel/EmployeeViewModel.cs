using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ToDoListManagement.Entity.ViewModel;

public class EmployeeViewModel
{
    public int? EmployeeId { get; set; }

    [Required(ErrorMessage = Constants.Constants.NameRequiredError)]
    [MaxLength(50)]
    public string? Name { get; set; }

    [Required(ErrorMessage = Constants.Constants.EmailRequiredError)]
    [MaxLength(50)]
    [EmailAddress(ErrorMessage = Constants.Constants.EmailInvalidError)]
    [Remote("CheckEmailExists", "Auth", ErrorMessage = Constants.Constants.EmailAlreadyExistsError)]
    public string? Email { get; set; }

    [Required(ErrorMessage = Constants.Constants.PasswordRequiredError)]
    [MaxLength(255)]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[.@$!%*?&])[A-Za-z\d.@$!%*?&]{8,}$", ErrorMessage = Constants.Constants.ValidPasswordMessage)]
    public string? Password { get; set; }

    [Required(ErrorMessage = Constants.Constants.RoleRequiredError)]
    public int Role { get; set; }

    public string? RoleName { get; set; }

    [Required(ErrorMessage = Constants.Constants.PhoneNumberRequiredError)]
    [MaxLength(10)]
    [RegularExpression(@"^([0-9]{10})$", ErrorMessage = Constants.Constants.PhoneNumberInvalidError)]
    public string? PhoneNumber { get; set; }

    public string? Status { get; set; }

    public List<RoleViewModel>? Roles { get; set; }

    public ProjectViewModel? Project { get; set; }

    public UserViewModel? ReportingPerson { get; set; }

}