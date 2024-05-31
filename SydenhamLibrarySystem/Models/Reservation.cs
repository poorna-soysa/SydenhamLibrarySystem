using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace SydenhamLibrarySystem.Models;

[Table("Reservation")]
public class Reservation

{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public bool IsDeleted { get; set; } = false;

    public DateTime? ReservationDate { get; set; } 
    public DateTime? IssuedDate { get; set; } 
    public DateTime? ReturnedDate { get; set; } 

    public string Status { get; set; } = string.Empty;

    public ICollection<ReservationDetail> ReservationDetails { get; set; }    
}
