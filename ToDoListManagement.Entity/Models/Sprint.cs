using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoListManagement.Entity.Models;

public class Sprint
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SprintId { get; set; }

    [StringLength(50)]
    public string? SprintName { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsDeleted { get; set; }

    public int ScrumMasterId { get; set; }

    public int ProjectId { get; set; }

    [ForeignKey("ScrumMasterId")]
    public virtual User? ScrumMaster { get; set; }

    [ForeignKey("ProjectId")]
    public virtual Project? Project { get; set; }
}