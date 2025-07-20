using System.ComponentModel.DataAnnotations;

namespace ToDoListManagement.Entity.ViewModel;

public class LeaveViewModel
{
    public int? LeaveId { get; set; }

    public int? RequestedUserId { get; set; }

    public int? ApprovalUserId { get; set; }

    [Required(ErrorMessage = Constants.Constants.NameRequiredError)]
    public string? RequestedName { get; set; }

    public string? ApprovalName { get; set; }

    [StringLength(1000)]
    public string? Reason { get; set; }

    [Required(ErrorMessage = Constants.Constants.StartDateRequiredError)]
    public string? StartDate { get; set; }

    [Required(ErrorMessage = Constants.Constants.EndDateRequiredError)]
    public string? EndDate { get; set; }

    public string? ReturnDate { get; set;}

    public int? Duration { get; set; }

    public string? ApprovedOn { get; set; }

    public string? Status { get; set; }

    public string? CommentByApproval { get; set; }

    [StringLength(15)]
    public string? PhoneNumber { get; set; }

    [StringLength(15)]
    public string? AlternatePhoneNumber { get; set; }

    public bool IsDeleted { get; set; }

    public string? CreatedAt { get; set; }

    public string? UpdatedAt { get; set; }

    public bool IsAvailableOnPhone { get; set; }
}
