using System.ComponentModel.DataAnnotations;

namespace ToDoListManagement.Entity.ViewModel;

public class ResetPasswordViewModel
{
    public string? Email { get; set; }
    public string? Token {get; set; }
    
    [Required(ErrorMessage = Constants.Constants.PasswordRequiredError)]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[.@$!%*?&])[A-Za-z\d.@$!%*?&]{8,}$",
    ErrorMessage = Constants.Constants.ValidPasswordMessage)]
    public string? NewPassword { get; set; }

    [Required(ErrorMessage = Constants.Constants.ConfirmPasswordRequiredError)]
    [Compare("NewPassword", ErrorMessage = Constants.Constants.PasswordMismatchError)]
    public string? ConfirmPassword { get; set; }
}
