using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static SydenhamLibrarySystem.Constants.Enums;

namespace SydenhamLibrarySystem.Repositories;

public interface IReservationRepository
{
    Task<int> AddItem(int reservationId, int qty);
    Task<int> RemoveItem(int reservationId);
    Task<Reservation> GetUserReservation();
    Task<int> GetReservationItemCount(string userId = "");
    Task<Reservation> GetReservation(string userId);
    Task<Reservation> GetReservationById(int id);
    Task<IEnumerable<Reservation>> GetReservations();

    Task<bool> DoReservation();
    Task<bool> DoCheckout(int reservationId);
    Task<bool> DoCheckIn(int reservationId);
}

public class ReservationRepository : IReservationRepository
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ReservationRepository(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<int> AddItem(int id, int qty)
    {
        string userId = GetUserId();
        using var transaction = _db.Database.BeginTransaction();
        try
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("user is not logged-in");

            var cart = await GetReservation(userId);

            if (cart is null)
            {
                cart = new Reservation
                {
                    UserId = userId
                };
                _db.Reservations.Add(cart);
            }

            _db.SaveChanges();

            // Get cart detail 
            var reservationItem = _db.ReservationDetails
                              .FirstOrDefault(a => a.ReservationId == cart.Id && a.LibraryItemId == id);

            if (reservationItem is not null)
            {
                reservationItem.Quantity += qty;
            }
            else
            {
                var book = _db.LibraryItems.Find(id);

                reservationItem = new ReservationDetail
                {
                    LibraryItemId = id,
                    ReservationId = cart.Id,
                    Quantity = qty,
                };

                _db.ReservationDetails.Add(reservationItem);
            }

            _db.SaveChanges();
            transaction.Commit();
        }
        catch (Exception ex)
        {
        }

        var reservationItemCount = await GetReservationItemCount(userId);
        return reservationItemCount;
    }


    public async Task<int> RemoveItem(int libraryItemId)
    {
        //using var transaction = _db.Database.BeginTransaction();
        string userId = GetUserId();
        try
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("user is not logged-in");

            var reservation = await GetReservation(userId);

            if (reservation is null)
                throw new InvalidOperationException("Invalid reservation");

            // reservation detail section
            var reservationItem = _db.ReservationDetails
                              .FirstOrDefault(a => a.ReservationId == reservation.Id
                              && a.LibraryItemId == libraryItemId);

            if (reservationItem is null)
            {
                throw new InvalidOperationException("Not items in reservation");
            }
            else if (reservationItem.Quantity == 1)
            {
                _db.ReservationDetails.Remove(reservationItem);
            }
            else
            {
                reservationItem.Quantity = reservationItem.Quantity - 1;
            }
            _db.SaveChanges();
        }
        catch (Exception ex)
        {
        }

        var reservationItemCount = await GetReservationItemCount(userId);
        return reservationItemCount;
    }

    public async Task<Reservation> GetUserReservation()
    {
        var userId = GetUserId();

        if (userId == null)
            throw new InvalidOperationException("Invalid user id");

        var reservation = await _db.Reservations
                              .Include(a => a.ReservationDetails)
                              .ThenInclude(a => a.LibraryItem)
                              .Include(a => a.ReservationDetails)
                              .ThenInclude(a => a.LibraryItem)
                              .ThenInclude(a => a.Subject)
                              .Where(a => a.UserId == userId && a.ReservationDate == null).FirstOrDefaultAsync();
        return reservation;

    }
    public async Task<Reservation> GetReservation(string userId)
    {
        var reservation = await _db.Reservations
            .Include(d=>d.ReservationDetails)
            .FirstOrDefaultAsync(x => x.UserId == userId 
            && x.ReservationDate == null);
        return reservation;
    }

    public async Task<Reservation> GetReservationById(int id)
    {
        var reservation = await _db.Reservations.FirstOrDefaultAsync(x => x.Id == id);
        return reservation;
    }

    public async Task<int> GetReservationItemCount(string userId = "")
    {
        if (string.IsNullOrEmpty(userId)) // updated line
        {
            userId = GetUserId();
        }
        var data = await (from r in _db.Reservations
                          join rd in _db.ReservationDetails
                          on r.Id equals rd.ReservationId
                          where r.UserId == userId && r.ReservationDate == null
                          select new { rd.Id }
                    ).ToListAsync();
        return data.Count;
    }

    public async Task<bool> DoCheckout(int reservationId)
    {
        using var transaction = _db.Database.BeginTransaction();

        try
        {

            // Get reservation by id
            var reservation = await GetReservationById(reservationId);

            if (reservation is null)
                throw new InvalidOperationException("Invalid Reservation");

            // Get reservation details from database
            var reservationDetail = _db.ReservationDetails
                                .Where(a => a.ReservationId == reservation.Id).ToList();

            if (reservationDetail.Count == 0)
                throw new InvalidOperationException("Reservation is empty");

            reservation.Status = ReservationStatus.Issued.ToString();
            reservation.IssuedDate = DateTime.Now;

            _db.SaveChanges();

            foreach (var item in reservationDetail)
            {
                // update library item no.of copies
                var libraryItem = await _db.LibraryItems.FirstOrDefaultAsync(a => a.Id == item.LibraryItemId);

                if (libraryItem == null)
                {
                    throw new InvalidOperationException("Library Item is null");
                }

                if (item.Quantity > libraryItem.NoOfCopies)
                {
                    throw new InvalidOperationException($"Only {libraryItem.NoOfCopies} items(s) are available in the stock");
                }

                // decrease the number of quantity from the book's no.of copies
                libraryItem.NoOfCopies -= item.Quantity;
            }

            _db.SaveChanges();
            transaction.Commit();

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> DoCheckIn(int reservationId)
    {
        using var transaction = _db.Database.BeginTransaction();

        try
        {

            // Get reservation by id
            var reservation = await GetReservationById(reservationId);

            if (reservation is null)
                throw new InvalidOperationException("Invalid Reservation");

            // Get reservation details from database
            var reservationDetail = _db.ReservationDetails
                                .Where(a => a.ReservationId == reservation.Id).ToList();

            if (reservationDetail.Count == 0)
                throw new InvalidOperationException("Reservation is empty");

            reservation.Status = ReservationStatus.Returned.ToString();
            reservation.ReturnedDate = DateTime.Now;

            _db.SaveChanges();

            foreach (var item in reservationDetail)
            {
                // update library item no.of copies
                var libraryItem = await _db.LibraryItems.FirstOrDefaultAsync(a => a.Id == item.LibraryItemId);

                if (libraryItem == null)
                {
                    throw new InvalidOperationException("Library Item is null");
                }

                if (item.Quantity > libraryItem.NoOfCopies)
                {
                    throw new InvalidOperationException($"Only {libraryItem.NoOfCopies} items(s) are available in the stock");
                }

                // decrease the number of quantity from the book's no.of copies
                libraryItem.NoOfCopies += item.Quantity;
            }

            _db.SaveChanges();
            transaction.Commit();

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private string GetUserId()
    {
        var principal = _httpContextAccessor.HttpContext.User;
        string userId = _userManager.GetUserId(principal);
        return userId;
    }

    public async Task<IEnumerable<Reservation>> GetReservations() => await _db.Reservations
        .Include(d => d.ReservationDetails)
      .ThenInclude(d => d.LibraryItem)
        .ThenInclude(d=>d.Subject)
        .Include(d=>d.User)
        .Where(d => d.ReservationDate != null).ToListAsync();

    public async Task<bool> DoReservation()
    {
        string userId = GetUserId();

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("user is not logged-in");

        var reservation = await _db.Reservations
            .FirstOrDefaultAsync(x => x.UserId == userId
            && x.ReservationDate == null);

        if (reservation == null)
            throw new InvalidOperationException("Invalid Reservation");

        reservation.Status = ReservationStatus.Pending.ToString();
        reservation.ReservationDate = DateTime.Now;

        await _db.SaveChangesAsync();
        return true;
    }
}
