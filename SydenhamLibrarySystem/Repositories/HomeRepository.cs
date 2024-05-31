using Microsoft.EntityFrameworkCore;

namespace SydenhamLibrarySystem.Repositories;

public interface IHomeRepository
{
    Task<IEnumerable<LibraryItem>> GetLibraryItems(string searchTerm = "", int categoryId = 0);
    Task<LibraryItem?> GetLibraryItem(int id);
    Task<IEnumerable<Subject>> Subjects();
}

public class HomeRepository : IHomeRepository
{
    private readonly ApplicationDbContext _context;

    public HomeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Subject>> Subjects()
    {
        return await _context.Subjects.ToListAsync();
    }
    public async Task<IEnumerable<LibraryItem>> GetLibraryItems(string searchTerm = "", 
        int subjectId = 0)
    {
        searchTerm = searchTerm.ToLower();
      
        var libraryItems = await _context.LibraryItems
            .Include(d => d.Subject)
            .Where(d => string.IsNullOrWhiteSpace(searchTerm)
                                         || d.Title.ToLower().StartsWith(searchTerm)
                                         || d.AuthorName.ToLower().Contains(searchTerm))
            .ToListAsync();
       
        if (subjectId > 0)
        {
            libraryItems = libraryItems.Where(a => a.SubjectId == subjectId).ToList();

        }
        return libraryItems;
    }

    public async Task<LibraryItem?> GetLibraryItem(int id)
    {
        var libraryItem = await _context.LibraryItems
            .Include(d => d.Subject)
            .Where(d => d.Id == id)
            .FirstOrDefaultAsync();

        return libraryItem;
    }
}
