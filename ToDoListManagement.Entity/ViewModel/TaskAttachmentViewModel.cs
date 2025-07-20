namespace ToDoListManagement.Entity.ViewModel;

public class TaskAttachmentViewModel
{
    public int TaskAttachmentId { get; set; }

    public int TaskId { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public bool IsImage { get; set; }

    public DateTime? UploadedOn { get; set; }

    public bool IsDeleted { get; set; }
}
