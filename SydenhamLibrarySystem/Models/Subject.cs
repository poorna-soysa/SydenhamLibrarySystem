using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SydenhamLibrarySystem.Models;

[Table("Subject")]
public class Subject
{
    public int Id { get; set; }

    [Required]
    [MaxLength(40)]
    public string SubjectName { get; set; }
    public List<LibraryItem> LibraryItems { get; set; }
}
