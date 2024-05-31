using System.ComponentModel.DataAnnotations;

namespace SydenhamLibrarySystem.Models.DTOs
{
    public class SubjectDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string SubjectName { get; set; } = string.Empty;
    }
}
