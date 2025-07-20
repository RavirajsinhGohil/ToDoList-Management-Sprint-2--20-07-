using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoListManagement.Entity.Models;

public class ErrorLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ErrorLogId { get; set; }

    public string? ErrorMessage { get; set; }

    public string? StackTrace { get; set; }
    
    public DateTime CreatedOn { get; set; } = DateTime.Now;
}