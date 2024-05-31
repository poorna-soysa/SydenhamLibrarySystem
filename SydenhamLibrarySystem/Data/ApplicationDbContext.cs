using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SydenhamLibrarySystem.Data;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Subject> Subjects { get; set; }
    public DbSet<LibraryItem> LibraryItems { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<ReservationDetail> ReservationDetails { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

    public DbSet<OrderStatus> OrderStatuses { get; set; }
    public DbSet<Message> Messages { get; set; }

}