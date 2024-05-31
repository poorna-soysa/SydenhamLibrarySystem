using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SydenhamLibrarySystem.Repositories;

public interface IMessageRepository
{
    Task AddMessage(Message message);
    Task<Message?> GetMessageById(int id);
    Task DeleteMessage(Message message);

    Task<IEnumerable<Message>> GetMessages();
    Task<IEnumerable<Message>> GetMessagesByUserId();
}
public class MessageRepository : IMessageRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MessageRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager)
    {
        _context = db;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task AddMessage(Message message)
    {
        message.UserId = GetUserId();
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
    }
   
    public async Task DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
        await _context.SaveChangesAsync();
    }

    public async Task<Message?> GetMessageById(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task<IEnumerable<Message>> GetMessages()
    {
        return await _context.Messages.ToListAsync();
    }

    public async Task<IEnumerable<Message>> GetMessagesByUserId()
    {
        var userId = GetUserId();

        if (userId == null)
            throw new InvalidOperationException("Invalid user id");

        return await _context.Messages.Where(d=>d.UserId == userId).ToListAsync();

    }

    private string GetUserId()
    {
        var principal = _httpContextAccessor.HttpContext.User;
        string userId = _userManager.GetUserId(principal);
        return userId;
    }
}
