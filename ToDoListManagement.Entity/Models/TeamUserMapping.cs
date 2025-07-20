using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoListManagement.Entity.Models;
public class TeamUserMapping
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TeamUserMappingId { get; set; }

    public int? TeamManagerId { get; set; }

    public int? UserId { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public DateOnly? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("TeamManagerId")]
    public virtual User? TeamManager { get; set; }

    [ForeignKey("UserId")]
    public virtual User? TeamMember { get; set; }
}