namespace ToDoListManagement.Entity.ViewModel;

public class ProjectTasksViewModel
{
    public int ProjectId { get; set; }
    public ProjectViewModel? ProjectView { get; set; }
    public List<TaskViewModel>? BacklogTasks { get; set; }
    public List<TaskViewModel>? Tasks { get; set; }
    public List<ProjectDropDown>? Projects { get; set; }
    public List<UserViewModel>? TeamMembers { get; set; }
    public List<SprintViewModel>? Sprints { get; set; }
}

public class ProjectDropDown{
    public int ProjectId { get; set; }
    public string? ProjectName { get; set; }
}
