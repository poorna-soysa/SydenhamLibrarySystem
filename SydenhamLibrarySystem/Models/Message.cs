using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SydenhamLibrarySystem.Models;

[Table("Message")]
public class Message
{
    public int Id { get; set; }

    [Required]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; } = DateTime.Now;

   
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
}
