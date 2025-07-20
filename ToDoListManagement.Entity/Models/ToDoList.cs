using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoListManagement.Entity.Models;

public partial class ToDoList
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TaskId { get; set; }

    public int? ProjectId { get; set; }

    public int? SprintId { get; set; }

    public int? CreatedBy { get; set; }

    public int? AssignedTo { get; set; }

    [StringLength(50)]
    public string? Title { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? DueDate { get; set; }

    [StringLength(20)]
    public string? Priority { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("ProjectId")]
    public virtual Project? Project { get; set; }

    [ForeignKey("SprintId")]
    public virtual Sprint? Sprint { get; set; }

    [ForeignKey("CreatedBy")]
    public virtual User? CreatedUser { get; set; }

    [ForeignKey("AssignedTo")]
    public virtual User? AssignedUser { get; set; }
}