using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SydenhamLibrarySystem.Models.DTOs;
public class LibraryItemDTO
{
    public int Id { get; set; }

    [Required]
    public string? Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    [MaxLength(40)]
    public string? AuthorName { get; set; }

    [Required]
    public string Type { get; set; } = string.Empty;

    [Required]
    public double Price { get; set; }
    public int NoOfCopies { get; set; }
    public string? Image { get; set; }
    [Required]
    public int SubjectId { get; set; }
    public IFormFile? ImageFile { get; set; }
    public IEnumerable<SelectListItem>? SubjectList { get; set; }
}
