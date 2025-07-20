namespace ToDoListManagement.Entity.ViewModel;

public class PermissionViewModel
{
    public int PermissionId { get; set; }

    public string? RoleName { get; set; }

    public int RoleId { get; set; }
    
    public string? PermissionName { get; set; }

    public bool CanView { get; set; }

    public bool CanAddEdit { get; set; }

    public bool CanDelete { get; set; }
}
