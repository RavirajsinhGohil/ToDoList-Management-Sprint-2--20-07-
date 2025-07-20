using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoListManagement.Entity.Models
{
    public partial class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [StringLength(50)]
        public string? Name { get; set; }

        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? PasswordHash { get; set; }

        public int RoleId { get; set; }

        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public int? ReportingPersonId { get; set; }

        public bool IsDeleted { get; set; } = false;

        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }

        [ForeignKey("ReportingPersonId")]
        public virtual User? ReportingPerson { get; set; }
    }
}