using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ToDoListManagement.Entity.ViewModel;

public class ProjectViewModel
{
    public int? ProjectId { get; set; }

    [Required(ErrorMessage = Constants.Constants.NameRequiredError)]
    [MaxLength(50)]
    [Remote("CheckProjectNameExists", "Project", AdditionalFields = nameof(ProjectId), ErrorMessage = Constants.Constants.NameExistsError)]
    public string? ProjectName { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required(ErrorMessage = Constants.Constants.StartDateRequiredError)]
    public DateTime? StartDate { get; set; }

    [Required(ErrorMessage = Constants.Constants.DueDateRequiredError)]
    public DateTime? EndDate { get; set; }

    public string? Status { get; set; }

    [Required(ErrorMessage = Constants.Constants.AssignedToPMRequiredError)]
    public int? AssignedToProgramManager { get; set; }

    [Required(ErrorMessage = Constants.Constants.AssignedToSMRequiredError)]
    public int? AssignedToScrumMaster { get; set; }

    public string? PMName { get; set; }

    public List<UserViewModel> ProjectManagers { get; set; } = [];

    public List<UserViewModel> ScrumMasters { get; set; } = [];
    
    public List<UserViewModel> Users { get; set; } = [];
}