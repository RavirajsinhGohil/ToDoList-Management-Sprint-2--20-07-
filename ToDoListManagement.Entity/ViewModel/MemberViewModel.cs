namespace ToDoListManagement.Entity.ViewModel;

public class MemberViewModel
{
    public int UserId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsAssigned { get; set; }
}