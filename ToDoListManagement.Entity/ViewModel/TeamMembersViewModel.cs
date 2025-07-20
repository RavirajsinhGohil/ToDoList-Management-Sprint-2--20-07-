using System.ComponentModel.DataAnnotations;

namespace ToDoListManagement.Entity.ViewModel;

public class TeamMembersViewModel
{
    public int? TeamManagerId { get; set; }

    [Required]
    public List<int> TeamMemberIds { get; set; } = [];
}
