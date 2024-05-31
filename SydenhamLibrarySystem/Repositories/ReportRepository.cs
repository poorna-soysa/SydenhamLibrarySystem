using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using static SydenhamLibrarySystem.Constants.Enums;

namespace SydenhamLibrarySystem.Repositories;

[Authorize(Roles = nameof(Roles.Admin))]
public class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _context;
    public ReportRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TopNSoldBookModel>> GetTopNSellingBooksByDate(DateTime startDate, DateTime endDate)
    {
        var topFiveSoldBooks = await (from od in _context.OrderDetails
                               where  !od.Order.IsDeleted && od.Order.CreateDate >= startDate
                               && od.Order.CreateDate < endDate
                               group od by od.BookId into unitSoldGroup
                               select new { BookId = unitSoldGroup.Key, TotalUnitSold = unitSoldGroup.Sum(od => od.Quantity) })
                        .OrderByDescending(d => d.TotalUnitSold)
                       .Take(5)
                       .Join(_context.LibraryItems, us => us.BookId, b => b.Id, (us, b) => new TopNSoldBookModel(b.Title, b.AuthorName, us.TotalUnitSold))
                     .ToListAsync();

        return topFiveSoldBooks;
    }

}

public interface IReportRepository
{
    Task<IEnumerable<TopNSoldBookModel>> GetTopNSellingBooksByDate(DateTime startDate, DateTime endDate);
}