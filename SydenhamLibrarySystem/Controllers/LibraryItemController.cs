using SydenhamLibrarySystem.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static SydenhamLibrarySystem.Constants.Enums;

namespace SydenhamLibrarySystem.Controllers;

[Authorize(Roles = nameof(Roles.Admin))]
public class LibraryItemController : Controller
{
    private readonly ILibraryItem _libraryItemRepo;
    private readonly ISubjectRepository _subjectRepo;
    private readonly IFileService _fileService;

    public LibraryItemController(ILibraryItem libraryItemRepo,
        ISubjectRepository subjectRepo,
        IFileService fileService)
    {
        _libraryItemRepo = libraryItemRepo;
        _subjectRepo = subjectRepo;
        _fileService = fileService;
    }

    public async Task<IActionResult> Index()
    {
        var books = await _libraryItemRepo.GetLibraryItems();
        return View(books);
    }

    public async Task<IActionResult> AddLibraryItem()
    {
        var subjectSelectList = (await _subjectRepo.GetSubject())
            .Select(subject => new SelectListItem
            {
                Text = subject.SubjectName,
                Value = subject.Id.ToString(),
            });

        LibraryItemDTO itemToAdd = new() { SubjectList = subjectSelectList };

        return View(itemToAdd);
    }

    [HttpPost]
    public async Task<IActionResult> AddLibraryItem(LibraryItemDTO itemToAdd)
    {
        var genreSelectList = (await _subjectRepo.GetSubject())
            .Select(subject => new SelectListItem
            {
                Text = subject.SubjectName,
                Value = subject.Id.ToString(),
            });

        itemToAdd.SubjectList = genreSelectList;

        if (!ModelState.IsValid)
            return View(itemToAdd);

        try
        {
            if (itemToAdd.ImageFile != null)
            {
                if (itemToAdd.ImageFile.Length > 1 * 1024 * 1024)
                {
                    throw new InvalidOperationException("Image file can not exceed 1 MB");
                }
                string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                string imageName = await _fileService.SaveFile(itemToAdd.ImageFile, allowedExtensions);
                itemToAdd.Image = imageName;
            }

            LibraryItem libraryItem = new()
            {
                Id = itemToAdd.Id,
                Title = itemToAdd.Title,
                AuthorName = itemToAdd.AuthorName,
                Image = itemToAdd.Image,
                Type = itemToAdd.Type,
                SubjectId = itemToAdd.SubjectId,
                Price = itemToAdd.Price,
                NoOfCopies = itemToAdd.NoOfCopies,
                Description = itemToAdd.Description,
            };

            await _libraryItemRepo.AddLibraryItem(libraryItem);

            TempData["successMessage"] = "Library Item is added successfully";

            return RedirectToAction(nameof(AddLibraryItem));
        }
        catch (InvalidOperationException ex)
        {
            TempData["errorMessage"] = ex.Message;
            return View(itemToAdd);
        }
        catch (FileNotFoundException ex)
        {
            TempData["errorMessage"] = ex.Message;
            return View(itemToAdd);
        }
        catch (Exception ex)
        {
            TempData["errorMessage"] = "Error on saving data";
            return View(itemToAdd);
        }
    }

    public async Task<IActionResult> UpdateLibraryItem(int id)
    {
        var libraryItem = await _libraryItemRepo.GetLibraryItemById(id);

        if (libraryItem == null)
        {
            TempData["errorMessage"] = $"Library Item with the id: {id} does not found";
            return RedirectToAction(nameof(Index));
        }
        var subjectSelectList = (await _subjectRepo.GetSubject())
            .Select(subject => new SelectListItem
            {
                Text = subject.SubjectName,
                Value = subject.Id.ToString(),
                Selected = subject.Id == libraryItem.SubjectId
            });

        LibraryItemDTO libraryItemToUpdate = new()
        {
            SubjectList = subjectSelectList,
            Title = libraryItem.Title,
            AuthorName = libraryItem.AuthorName,
            SubjectId = libraryItem.SubjectId,
            Price = libraryItem.Price,
            Image = libraryItem.Image,
            Type = libraryItem.Type,
            Description = libraryItem.Description,
            NoOfCopies = libraryItem.NoOfCopies,
        };
        return View(libraryItemToUpdate);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateLibraryItem(LibraryItemDTO libraryItemRequest)
    {
        var categorySelectList = (await _subjectRepo.GetSubject())
            .Select(s => new SelectListItem
            {
                Text = s.SubjectName,
                Value = s.Id.ToString(),
                Selected = s.Id == libraryItemRequest.SubjectId
            });

        libraryItemRequest.SubjectList = categorySelectList;

        if (!ModelState.IsValid)
            return View(libraryItemRequest);

        try
        {
            string oldImage = "";
            if (libraryItemRequest.ImageFile != null)
            {
                if (libraryItemRequest.ImageFile.Length > 1 * 1024 * 1024)
                {
                    throw new InvalidOperationException("Image file can not exceed 1 MB");
                }
                string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                string imageName = await _fileService.SaveFile(libraryItemRequest.ImageFile, allowedExtensions);

                oldImage = libraryItemRequest.Image;
                libraryItemRequest.Image = imageName;
            }


            LibraryItem libraryItem = new()
            {
                Id = libraryItemRequest.Id,
                Title = libraryItemRequest.Title,
                AuthorName = libraryItemRequest.AuthorName,
                SubjectId = libraryItemRequest.SubjectId,
                Price = libraryItemRequest.Price,
                Type = libraryItemRequest.Type,
                Image = libraryItemRequest.Image,
                NoOfCopies = libraryItemRequest.NoOfCopies,
                Description = libraryItemRequest.Description,
            };

            await _libraryItemRepo.UpdateLibraryItem(libraryItem);

            // if image is updated, then delete it from the folder too
            if (!string.IsNullOrWhiteSpace(oldImage))
            {
                _fileService.DeleteFile(oldImage);
            }

            TempData["successMessage"] = "Library Item is updated successfully";
            
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            TempData["errorMessage"] = ex.Message;
            return View(libraryItemRequest);
        }
        catch (FileNotFoundException ex)
        {
            TempData["errorMessage"] = ex.Message;
            return View(libraryItemRequest);
        }
        catch (Exception ex)
        {
            TempData["errorMessage"] = "Error on saving data";
            return View(libraryItemRequest);
        }
    }

    public async Task<IActionResult> DeleteLibraryItem(int id)
    {
        try
        {
            var libraryItem = await _libraryItemRepo.GetLibraryItemById(id);
           
            if (libraryItem == null)
            {
                TempData["errorMessage"] = $"library Item with the id: {id} does not found";
            }
            else
            {
                await _libraryItemRepo.DeleteLibraryItem(libraryItem);
               
                if (!string.IsNullOrWhiteSpace(libraryItem.Image))
                {
                    _fileService.DeleteFile(libraryItem.Image);
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            TempData["errorMessage"] = ex.Message;
        }
        catch (FileNotFoundException ex)
        {
            TempData["errorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            TempData["errorMessage"] = "Error on deleting the data";
        }
        return RedirectToAction(nameof(Index));
    }

}
