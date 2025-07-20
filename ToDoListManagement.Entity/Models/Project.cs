using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoListManagement.Entity.Models;

public partial class Project
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProjectId { get; set; }

    [StringLength(50)]
    public string? ProjectName { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedOn { get; set; }

    public DateTime? EndDate { get; set; }
    
    [StringLength(1000)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }

    public int? AssignedToProgramManager { get; set; }

    public int? AssignedToScrumMaster { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("CreatedBy")]
    public virtual User? User { get; set; }

    [ForeignKey("AssignedToProgramManager")]
    public virtual User? AssignedPM { get; set; }

    [ForeignKey("AssignedToScrumMaster")]
    public virtual User? ScrumMaster { get; set; }

    public virtual ICollection<ProjectUser>? ProjectUsers { get; set; } = [];
}