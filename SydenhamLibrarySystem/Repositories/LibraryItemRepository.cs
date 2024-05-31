using Microsoft.EntityFrameworkCore;

namespace SydenhamLibrarySystem.Repositories
{
    public interface ILibraryItem
    {
        Task AddLibraryItem(LibraryItem libraryItem);
        Task DeleteLibraryItem(LibraryItem libraryItem);
        Task<LibraryItem?> GetLibraryItemById(int id);
        Task<IEnumerable<LibraryItem>> GetLibraryItems();
        Task UpdateLibraryItem(LibraryItem libraryItem);
    }

    public class LibraryItemRepository : ILibraryItem
    {
        private readonly ApplicationDbContext _context;
        public LibraryItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddLibraryItem(LibraryItem libraryItem)
        {
            _context.LibraryItems.Add(libraryItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateLibraryItem(LibraryItem libraryItem)
        {
            _context.LibraryItems.Update(libraryItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLibraryItem(LibraryItem libraryItem)
        {
            _context.LibraryItems.Remove(libraryItem);
            await _context.SaveChangesAsync();
        }

        public async Task<LibraryItem?> GetLibraryItemById(int id) => await _context.LibraryItems.FindAsync(id);

        public async Task<IEnumerable<LibraryItem>> GetLibraryItems() => await _context.LibraryItems
            .Include(a=>a.Subject)
            .ToListAsync();
    }
}
