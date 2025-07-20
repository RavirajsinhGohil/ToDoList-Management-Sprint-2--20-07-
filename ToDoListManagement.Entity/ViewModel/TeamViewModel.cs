namespace ToDoListManagement.Entity.ViewModel;

public class TeamViewModel
{
    public UserViewModel TeamMember { get; set; } = new();
    
    public List<UserViewModel> TeamMembers { get; set; } = [];
    
    public UserViewModel TeamManager { get; set; } = new();

    public List<UserViewModel> TeamManagers { get; set; } = [];

    public int? SelectedTeamManagerId { get; set; }    
}