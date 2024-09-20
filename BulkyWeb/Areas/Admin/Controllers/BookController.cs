using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using BulkyWeb.Areas.Admin.ViewModels;
using BulkyWeb.Areas.Admin.ViewModels.Books;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReadOnlyRepository<BookListViewModel> _readOnlyRepository;

        public BookController(IUnitOfWork unitOfWork, IReadOnlyRepository<BookListViewModel> readOnlyRepository)
        {
            _unitOfWork = unitOfWork;
            _readOnlyRepository = readOnlyRepository;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var lstBooks = await _readOnlyRepository.GetAllAsync();

                return View(lstBooks);
            }
            catch (Exception ex)
            {
                // Log exception (ex) here
                TempData["error"] = "An error occurred while retrieving the books.";
                return View("Error");
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                List<CategoryViewModel> categories = (await _unitOfWork.Category.GetAllOrderedByDisplayOrderAsync())
                    .Select(x => new CategoryViewModel 
                    {
                        Id = x.Id,
                        Name = x.Name 
                    }).ToList();

                var bookViewModel = new AddEditBookViewModel
                {
                    Categories = categories
                };

                return View(bookViewModel);
            }
            catch (Exception ex)
            {
                // Log exception (ex) here
                TempData["error"] = "An error occurred while retrieving the categories.";
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddEditBookViewModel bookViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TbBook bookModel = new();

                    Mapper(bookViewModel, bookModel);

                    await _unitOfWork.Book.AddAsync(bookModel);

                    await _unitOfWork.SaveAsync();
                    TempData["success"] = "Book created successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Log exception (ex) here
                    TempData["error"] = "An error occurred while creating the book.";
                    return View("Error");
                }
            }

            return View(bookViewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return NotFound();

            try
            {
                var bookModel = await _unitOfWork.Book.GetByIdAsync(id);

                if (bookModel == null)
                    return NotFound();

                AddEditBookViewModel bookViewModel = new();
                Mapper(bookModel, bookViewModel);

                return View(bookViewModel);
            }
            catch (Exception ex)
            {
                // Log exception (ex) here
                TempData["error"] = "An error occurred while retrieving the book for editing.";
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AddEditBookViewModel bookViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var produxtModel = await _unitOfWork.Book.GetByIdAsync(id);

                    if (produxtModel == null)
                        return NotFound();

                    Mapper(bookViewModel, produxtModel);

                    _unitOfWork.Book.Update(produxtModel);
                    await _unitOfWork.SaveAsync();
                    TempData["success"] = "Book updated successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Log exception (ex) here
                    TempData["error"] = "An error occurred while updating the book.";
                    return View("Error");
                }
            }

            return View(bookViewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return NotFound();

            try
            {
                var bookModel = await _unitOfWork.Book.GetByIdAsync(id);

                if (bookModel == null)
                    return NotFound();

                BookDetailsViewModel bookDetailsViewModel = new();

                Mapper(bookModel, bookDetailsViewModel);

                return View(bookDetailsViewModel);
            }
            catch (Exception ex)
            {
                // Log exception (ex) here
                TempData["error"] = "An error occurred while retrieving the book for deletion.";
                return View("Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int id)
        {
            if (id <= 0)
                return NotFound();

            try
            {
                var book = await _unitOfWork.Book.GetByIdAsync(id);

                if (book == null)
                    return NotFound();

                _unitOfWork.Book.Remove(book);
                await _unitOfWork.SaveAsync();
                TempData["success"] = "Book deleted successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log exception (ex) here
                TempData["error"] = "An error occurred while deleting the book.";
                return View("Error");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return NotFound();

            try
            {
                var bookModel = await _unitOfWork.Book.GetByIdAsync(id);

                if (bookModel == null)
                    return NotFound();

                BookDetailsViewModel bookDetailsViewModel = new();

                Mapper(bookModel, bookDetailsViewModel);

                return View(bookDetailsViewModel);
            }
            catch (Exception ex)
            {
                // Log exception (ex) here
                TempData["error"] = "An error occurred while retrieving book details.";

                /* ErrorViewModel model = new ErrorViewModel() { RequestId = Guid.NewGuid().ToString() };
                 return View("Error", model);*/

                return View("Error");
            }
        }















        private static void Mapper(TbBook bookModel, AddEditBookViewModel bookViewModel)
        {
            bookViewModel.Id = bookModel.Id;
            bookViewModel.Title = bookModel.Title;
            bookViewModel.Description = bookModel.Description;
            bookViewModel.ISBN = bookModel.ISBN;
            bookViewModel.Author = bookModel.Author;
            bookViewModel.ListPrice = bookModel.ListPrice;
            bookViewModel.Price = bookModel.Price;
            bookViewModel.Price50 = bookModel.Price50;
            bookViewModel.Price100 = bookModel.Price100;
        }

/*        private static void Mapper(TbBook bookModel, BookListViewModel bookListViewModel)
        {
            bookListViewModel.Id = bookModel.Id;
            bookListViewModel.Title = bookModel.Title;
            bookListViewModel.ISBN = bookModel.ISBN;
            bookListViewModel.Author = bookModel.Author;
            bookListViewModel.ListPrice = bookModel.ListPrice;
        }*/

        private static void Mapper(AddEditBookViewModel bookViewModel, TbBook bookModel)
        {
            bookModel.Title = bookViewModel.Title;
            bookModel.Description = bookViewModel.Description;
            bookModel.ISBN = bookViewModel.ISBN;
            bookModel.Author = bookViewModel.Author;
            bookModel.ListPrice = bookViewModel.ListPrice;
            bookModel.Price = bookViewModel.Price;
            bookModel.Price50 = bookViewModel.Price50;
            bookModel.Price100 = bookViewModel.Price100;
            bookModel.CategoryId = bookViewModel.CategoryId;
            bookModel.ImageUrl = "";
        }

        private static void Mapper(TbBook bookModel, BookDetailsViewModel bookDetailsViewModel)
        {
            bookDetailsViewModel.Id = bookModel.Id;
            bookDetailsViewModel.Title = bookModel.Title;
            bookDetailsViewModel.Description = bookModel.Description;
            bookDetailsViewModel.ISBN = bookModel.ISBN;
            bookDetailsViewModel.Author = bookModel.Author;
            bookDetailsViewModel.ListPrice = bookModel.ListPrice;
            bookDetailsViewModel.Price = bookModel.Price;
            bookDetailsViewModel.Price50 = bookModel.Price50;
            bookDetailsViewModel.Price100 = bookModel.Price100;
        }
    }
}
