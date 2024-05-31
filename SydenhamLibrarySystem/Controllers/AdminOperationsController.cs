using SydenhamLibrarySystem.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static SydenhamLibrarySystem.Constants.Enums;

namespace SydenhamLibrarySystem.Controllers;

[Authorize(Roles = nameof(Roles.Admin))]
public class AdminOperationsController : Controller
{
    private readonly IReservationRepository _reservationRepository;
    public AdminOperationsController(        IReservationRepository reservationRepository)
    {
        _reservationRepository = reservationRepository;
    }

    public async Task<IActionResult> AllReservation()
    {
        var reservations = await _reservationRepository.GetReservations();
        return View(reservations);
    }

   

    public async Task<IActionResult> DoCheckout(int reservationId)
    {
        var isSuccess = await _reservationRepository.DoCheckout(reservationId);
        
        if (!isSuccess)
        {
            throw new InvalidOperationException($"Reservation with id:{reservationId} does not found.");
        }

        return RedirectToAction(nameof(AllReservation));
    }

    public async Task<IActionResult> DoCheckIn(int reservationId)
    {
        var isSuccess = await _reservationRepository.DoCheckIn(reservationId);

        if (!isSuccess)
        {
            throw new InvalidOperationException($"Reservation with id:{reservationId} does not found.");
        }

        return RedirectToAction(nameof(AllReservation));
    }

   


    public IActionResult Dashboard()
    {
        
        return View();
    }

}
