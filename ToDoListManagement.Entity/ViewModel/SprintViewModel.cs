using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ToDoListManagement.Entity.ViewModel;

public class SprintViewModel
{
    public int SprintId { get; set; }
    
    [Required(ErrorMessage = Constants.Constants.NameRequiredError)]
    [MaxLength(50)]
    [Remote("CheckSprintNameExists", "Tasks", AdditionalFields = nameof(ProjectId), ErrorMessage = Constants.Constants.NameExistsError)]
    public string? Name { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime ProjectStartDate { get; set; }

    public DateTime ProjectEndDate { get; set; }

    public int? Duration
    {
        get
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                return (int)(EndDate.Value - StartDate.Value).TotalDays;
            }
            return 10;
        }
    }

    public int? ScrumMasterId { get; set; }
    
    public int ProjectId { get; set; }

    public int? Progress { get; set; }
}