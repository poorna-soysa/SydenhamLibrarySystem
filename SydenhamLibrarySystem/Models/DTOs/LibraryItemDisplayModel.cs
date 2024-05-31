namespace SydenhamLibrarySystem.Models.DTOs;

public class LibraryItemDisplayModel
{
    public IEnumerable<LibraryItem> LibraryItems { get; set; }
    public IEnumerable<Subject> Subjects { get; set; }
    public string SearchTerm { get; set; } = string.Empty;
    public int SubjectId { get; set; } = 0;
}
