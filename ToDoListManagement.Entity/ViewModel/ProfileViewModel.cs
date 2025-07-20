using System.ComponentModel.DataAnnotations;

namespace ToDoListManagement.Entity.ViewModel;

public class ProfileViewModel
{
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = Constants.Constants.NameRequiredError)]
    [StringLength(100, ErrorMessage = Constants.Constants.NameMaxLengthError)]
    public string? Name { get; set; }

    public string? Email { get; set; }

    public int? Role { get; set; }

    public string? RoleName { get; set; }

    [Required(ErrorMessage = Constants.Constants.PhoneNumberRequiredError)]
    [RegularExpression(@"^([0-9]{10})$", ErrorMessage = Constants.Constants.PhoneNumberInvalidError)]
    public string? PhoneNumber { get; set; }
}