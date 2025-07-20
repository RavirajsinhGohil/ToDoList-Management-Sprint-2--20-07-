using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoListManagement.Entity.Models;

public class Role
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RoleId { get; set; }

    [StringLength(50)]
    public string? RoleName { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }
    
    public bool IsDeleted { get; set; }

    public virtual ICollection<Permission> Permissions { get; set; } = [];

    public virtual ICollection<User> Users { get; set; } = [];

    [NotMapped]
    public virtual User? CreatedByNavigation { get; set; }

    [NotMapped]
    public virtual User? UpdatedByNavigation { get; set; }

}