using Microsoft.EntityFrameworkCore;

namespace SydenhamLibrarySystem.Repositories;

public interface ISubjectRepository
{
    Task AddSubject(Subject subject);
    Task UpdateSubject(Subject subject);
    Task<Subject?> GetSubjectById(int id);
    Task DeleteSubject(Subject genre);
    Task<IEnumerable<Subject>> GetSubject();
}
public class SubjectRepository : ISubjectRepository
{
    private readonly ApplicationDbContext _context;
    public SubjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task AddSubject(Subject subject)
    {
        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateSubject(Subject subject)
    {
        _context.Subjects.Update(subject);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteSubject(Subject subject)
    {
        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();
    }

    public async Task<Subject?> GetSubjectById(int id)
    {
        return await _context.Subjects.FindAsync(id);
    }

    public async Task<IEnumerable<Subject>> GetSubject()
    {
        return await _context.Subjects.ToListAsync();
    }
}
