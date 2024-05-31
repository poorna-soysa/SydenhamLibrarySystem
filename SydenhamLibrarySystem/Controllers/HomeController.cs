using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace SydenhamLibrarySystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository _homeRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly UserManager<ApplicationUser> _userManager;


        public HomeController(ILogger<HomeController> logger,
            IHomeRepository homeRepository,
            UserManager<ApplicationUser> userManager,
            IMessageRepository messageRepository)
        {
            _homeRepository = homeRepository;
            _logger = logger;
            _userManager = userManager;
            _messageRepository = messageRepository;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
        public async Task<IActionResult> Book(string search = "", int categoryId = 0)
        {
            IEnumerable<LibraryItem> books = await _homeRepository.GetLibraryItems(search, categoryId);
            IEnumerable<Subject> categories = await _homeRepository.Subjects();
           
            LibraryItemDisplayModel bookModel = new LibraryItemDisplayModel
            {
                LibraryItems = books,
                Subjects = categories,
                SearchTerm = search,
                SubjectId = categoryId
            };
            return View(bookModel);
        }

        public async Task<IActionResult> AllMessage()
        {
            var messages = await _messageRepository.GetMessages();
           
            return View(messages);
        }

        public IActionResult AddMessage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddMessage(MessageDTO message)
        {
            if (!ModelState.IsValid)
            {
                return View(message);
            }

            try
            {
                await _messageRepository.AddMessage(new Message
                {
                    Body = message.Body,
                    Subject = message.Subject,
                });

                TempData["successMessage"] = "Message added successfully";

                return RedirectToAction(nameof(AllMessage));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Message could not added!";
                return View(message);
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _homeRepository.GetLibraryItem((int)id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> Details(int id, string review)
        {
            if (id == null)
            {
                return NotFound();
            }
           

            var book = await _homeRepository.GetLibraryItem((int)id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string GetUserId()
        {
            var principal = HttpContext.User;
            string userId = _userManager.GetUserId(principal);
            return userId;
        }
    }
}