using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ToDoListManagement.Entity.ViewModel;

public class TaskViewModel
{
    public int? TaskId { get; set; }

    public int? ProjectId { get; set; }

    public int? AssignedTo { get; set; }

    [Required(ErrorMessage = Constants.Constants.TitleRequiredError)]
    [MaxLength(50)]
    [Remote("CheckTaskTitleExists", "Tasks", AdditionalFields = "TaskId,ProjectId", ErrorMessage = Constants.Constants.TitleExistsError)]
    public string? Title { get; set; }
    
    [MaxLength(1000)]
    public string? Description { get; set; }

    public int? SprintId { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedOn { get; set; }

    public IFormFileCollection? TaskDetails { get; set; }

    public DateTime? DueDate { get; set; }

    [Required(ErrorMessage = Constants.Constants.PriorityRequiredError)]
    public string? Priority { get; set; }

    public List<UserViewModel> TeamMembers { get; set; } = [];

    public List<TaskAttachmentViewModel> TaskAttachments { get; set; } = [];

    public DateTime ProjectStartDate { get; set; }
    
    public DateTime ProjectEndDate { get; set; }

    public List<SprintViewModel> Sprints { get; set; } = [];
}
