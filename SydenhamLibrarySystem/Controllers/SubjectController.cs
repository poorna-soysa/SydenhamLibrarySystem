using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static SydenhamLibrarySystem.Constants.Enums;

namespace SydenhamLibrarySystem.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class SubjectController : Controller
    {
        private readonly ISubjectRepository _subjectRepository;

        public SubjectController(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task<IActionResult> Index()
        {
            var genres = await _subjectRepository.GetSubject();
            return View(genres);
        }

        public IActionResult AddSubject()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddSubject(SubjectDTO subject)
        {
            if(!ModelState.IsValid)
            {
                return View(subject);
            }

            try
            {
                var subjectToAdd = new Subject { SubjectName = subject.SubjectName, Id = subject.Id };
                
                await _subjectRepository.AddSubject(subjectToAdd);

                TempData["successMessage"] = "Subject added successfully";

                return RedirectToAction(nameof(AddSubject));
            }
            catch(Exception ex)
            {
                TempData["errorMessage"] = "Subject could not added!";
                return View(subject);
            }
        }

        public async Task<IActionResult> UpdateSubject(int id)
        {
            var subject = await _subjectRepository.GetSubjectById(id);
            
            if (subject is null)
                throw new InvalidOperationException($"Subject with id: {id} does not found");
            
            var categoryToUpdate = new SubjectDTO
            {
                Id = subject.Id,
                SubjectName = subject.SubjectName
            };

            return View(categoryToUpdate);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSubject(SubjectDTO subjectRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(subjectRequest);
            }

            try
            {
                var subject = new Subject { SubjectName = subjectRequest.SubjectName, Id = subjectRequest.Id };
               
                await _subjectRepository.UpdateSubject(subject);
                
                TempData["successMessage"] = "Subject is updated successfully";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Subject could not updated!";
                return View(subjectRequest);
            }

        }

        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _subjectRepository.GetSubjectById(id);

            if (subject is null)
                throw new InvalidOperationException($"Subject with id: {id} does not found");
            
            await _subjectRepository.DeleteSubject(subject);
            return RedirectToAction(nameof(Index));

        }
    }
}
