using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace SydenhamLibrarySystem.Models
{
    [Table("ReservationDetail")]
    public class ReservationDetail
    {
        public int Id { get; set; }

        [Required]
        public int ReservationId { get; set; }

        [Required]
        public int LibraryItemId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public LibraryItem LibraryItem { get; set; }
        public Reservation Reservation { get; set; }
    }
}
