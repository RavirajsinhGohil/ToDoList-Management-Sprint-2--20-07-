using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ToDoListManagement.Entity.ViewModel;

public class RegisterViewModel
{
    [Required(ErrorMessage = Constants.Constants.NameRequiredError)]
    [StringLength(100, ErrorMessage = Constants.Constants.NameMaxLengthError)]
    public string? Name { get; set; }

    [Required(ErrorMessage = Constants.Constants.EmailRequiredError)]
    [EmailAddress(ErrorMessage = Constants.Constants.EmailInvalidError)]
    [Remote("CheckEmailExists", "Auth", ErrorMessage = Constants.Constants.EmailAlreadyExistsError)]
    public string? Email { get; set; }

    [Required(ErrorMessage = Constants.Constants.PasswordRequiredError)]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[.@$!%*?&])[A-Za-z\d.@$!%*?&]{8,}$",
    ErrorMessage = Constants.Constants.ValidPasswordMessage)]
    public string? Password { get; set; }
}
