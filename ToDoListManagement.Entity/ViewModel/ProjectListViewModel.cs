namespace ToDoListManagement.Entity.ViewModel;

public class ProjectListViewModel
{
    public List<ProjectViewModel> Projects { get; set; } = [];

    public List<UserViewModel> ProjectManagers { get; set; } = [];
}
