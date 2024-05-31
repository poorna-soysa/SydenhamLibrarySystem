using System.ComponentModel.DataAnnotations;

namespace SydenhamLibrarySystem.Models;

public class MessageDTO
{
    [Required]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Body { get; set; } = string.Empty;
}
