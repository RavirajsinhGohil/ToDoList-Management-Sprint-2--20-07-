namespace ToDoListManagement.Entity.ViewModel;

public class AssignMembersViewModel
{
    public int ProjectId { get; set; }
    public List<int>? UserIds { get; set; } 
}
