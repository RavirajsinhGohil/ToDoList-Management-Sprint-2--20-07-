using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoListManagement.Entity.Models;

public partial class TaskAttachment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AttachmentId { get; set; }

    public int? TaskId { get; set; }

    [StringLength(255)]
    public string? FileName { get; set; }

    [StringLength(500)]
    public string? FilePath { get; set; }

    public bool? IsImage { get; set; }

    public DateTime? UploadedOn { get; set; }
    
    public int? UploadedBy { get; set; }

    public bool IsDeleted { get; set; } = false;

    public int? DeletedBy { get; set; }

    [ForeignKey("TaskId")]
    public virtual ToDoList? ToDoList { get; set; }

    [ForeignKey("UploadedBy")]
    public virtual User? UploadedByUser { get; set; }

    [ForeignKey("DeletedBy")]
    public virtual User? DeletedByUser { get; set; }
}