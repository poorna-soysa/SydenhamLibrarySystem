using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SydenhamLibrarySystem.Models
{
    [Table("LibraryItem")]
    public class LibraryItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(40)]
        public string AuthorName { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        public double Price { get; set; }
        public string? Image { get; set; }       

        public int NoOfCopies { get; set; }

        [Required]
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        [NotMapped]
        public string SubjectName { get; set; } = string.Empty;
    }
}
