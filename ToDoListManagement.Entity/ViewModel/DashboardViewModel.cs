namespace ToDoListManagement.Entity.ViewModel;

public class DashboardViewModel
{
    public List<ProjectViewModel> Projects { get; set; } = [];
    public int TotalNoOfProjects { get; set; }
    public int TotalNoOfLeaves { get; set; }
    public int TotalNoOfTasks { get; set; }
    public int TotalNoOfUsers { get; set; }
}
