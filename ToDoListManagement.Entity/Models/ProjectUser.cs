using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoListManagement.Entity.Models;

public partial class ProjectUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProjectUserId { get; set; }

    public int? ProjectId { get; set; }

    public int? UserId { get; set; }

    [ForeignKey("ProjectId")]
    public virtual Project? Project { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}