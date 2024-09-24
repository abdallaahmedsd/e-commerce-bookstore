using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels.Admin;
using Bulky.Models.ViewModels.Admin.Books;
using Bulky.Models.ViewModels.Customer;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area(SD.Role_Admin)]
    [Authorize(Roles  = SD.Role_Admin)]
	public class BookController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IReadOnlyRepository<BookListViewModel> _readOnlyRepository;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public BookController(IUnitOfWork unitOfWork, IReadOnlyRepository<BookListViewModel> readOnlyRepository, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_readOnlyRepository = readOnlyRepository;
			_webHostEnvironment = webHostEnvironment;
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
		public async Task<IActionResult> Create(AddEditBookViewModel bookViewModel, IFormFile file)
		{
			if (ModelState.IsValid)
			{
				try
				{
					string wwwRootPath = _webHostEnvironment.WebRootPath;
					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
					string bookPath = Path.Combine(wwwRootPath, @"uploads\images\books\");

					using (var fileStream = new FileStream(Path.Combine(bookPath, fileName), FileMode.Create))
					{
						await file.CopyToAsync(fileStream);
					}

					string imageUrl = @"uploads\images\books\" + fileName;

					TbBook bookModel = new();
					bookModel.ImageUrl = imageUrl;
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

			List<CategoryViewModel> categories = (await _unitOfWork.Category.GetAllOrderedByDisplayOrderAsync())
				.Select(x => new CategoryViewModel
				{
					Id = x.Id,
					Name = x.Name
				}).ToList();

			bookViewModel.Categories = categories;

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

				List<CategoryViewModel> categories = (await _unitOfWork.Category.GetAllOrderedByDisplayOrderAsync())
					.Select(x => new CategoryViewModel
					{
						Id = x.Id,
						Name = x.Name
					}).ToList();

				bookViewModel.Categories = categories;

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
		public async Task<IActionResult> Edit(int id, AddEditBookViewModel bookViewModel, IFormFile? file)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var bookModel = await _unitOfWork.Book.GetByIdAsync(id);

					if (bookModel == null)
						return NotFound();

					if (file != null)
					{
						// delete old image if the user selected a new image
						string wwwRootPath = _webHostEnvironment.WebRootPath;
						string oldImagePath = Path.Combine(wwwRootPath, bookModel.ImageUrl);
						if (System.IO.File.Exists(oldImagePath))
						{
							System.IO.File.Delete(oldImagePath);
						}

						// save the new image
						string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
						string bookPath = Path.Combine(wwwRootPath, @"uploads\images\books\");

						using (var fileStream = new FileStream(Path.Combine(bookPath, fileName), FileMode.Create))
						{
							await file.CopyToAsync(fileStream);
						}

						// update imageUrl
						string imageUrl = @"uploads\images\books\" + fileName;
						bookModel.ImageUrl = imageUrl;
					}

					Mapper(bookViewModel, bookModel);

					_unitOfWork.Book.Update(bookModel);
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

			List<CategoryViewModel> categories = (await _unitOfWork.Category.GetAllOrderedByDisplayOrderAsync())
				.Select(x => new CategoryViewModel
				{
					Id = x.Id,
					Name = x.Name
				}).ToList();

			bookViewModel.Categories = categories;

			return View(bookViewModel);
		}

		public async Task<IActionResult> Delete(int id)
		{
			if (id <= 0)
				return NotFound();

			try
			{
				var bookModel = await _unitOfWork.Book.GetByIdAsync(id, "Category");

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

		public async Task<IActionResult> Details(int id)
		{
			if (id <= 0)
				return NotFound();

			try
			{
				var bookModel = await _unitOfWork.Book.GetByIdAsync(id, "Category");

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
			bookViewModel.ImageUrl = bookModel.ImageUrl;
			bookViewModel.CategoryId = bookModel.CategoryId;
		}

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
			// bookModel.ImageUrl = "";
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
			bookDetailsViewModel.ImageUrl = bookModel.ImageUrl;
			bookDetailsViewModel.Category = bookModel.Category.Name;
		}
	}
}
