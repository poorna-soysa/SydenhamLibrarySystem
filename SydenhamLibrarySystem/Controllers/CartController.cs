using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SydenhamLibrarySystem.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IReservationRepository _cartRepo;

        public CartController(IReservationRepository cartRepo)
        {
            _cartRepo = cartRepo;
        }
        public async Task<IActionResult> AddItem(int id, int qty = 1, int redirect = 0)
        {
            var cartCount = await _cartRepo.AddItem(id, qty);
            if (redirect == 0)
                return Ok(cartCount);
            return RedirectToAction("GetUserCart");
        }

        public async Task<IActionResult> RemoveItem(int id)
        {
            var cartCount = await _cartRepo.RemoveItem(id);
            return RedirectToAction("GetUserCart");
        }
        public async Task<IActionResult> GetUserCart()
        {
            var cart = await _cartRepo.GetUserReservation();
            return View(cart);
        }

        public async Task<IActionResult> GetTotalItemInCart()
        {
            int cartItem = await _cartRepo.GetReservationItemCount();
            return Ok(cartItem);
        }

        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DoReservation()
        {
            var isSuccess = await _cartRepo.DoReservation();

            if (isSuccess)
            {
                return RedirectToAction(nameof(ReservationSuccess));

            }
            return RedirectToAction(nameof(ReservationFailure));
        }

        public IActionResult ReservationSuccess()
        {
            return View();
        }

        public IActionResult ReservationFailure()
        {
            return View();
        }

    }
}
