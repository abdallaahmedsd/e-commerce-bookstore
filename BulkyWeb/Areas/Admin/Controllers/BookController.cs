using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels.Admin;
using Bulky.Models.ViewModels.Admin.Books;
using Bulky.Utility;
using BulkyWeb.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area(SD.Role_Admin)]
	[Authorize(Roles = SD.Role_Admin)]
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

		public IActionResult Index()
		{
			return View();
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
		public async Task<IActionResult> Create(AddEditBookViewModel bookViewModel, IFormFile mainImage, List<IFormFile> files)
		{
			if (ModelState.IsValid)
			{
				try
				{
					TbBook bookModel = new();
					Mapper.Map(bookViewModel, bookModel);

					await _unitOfWork.Book.AddAsync(bookModel);

					await _unitOfWork.SaveAsync();

					#region Save book iamges 
					await _HandleBookImages(bookModel.Id, bookViewModel, mainImage, files);

					bookModel.BookImages = bookViewModel.BookImages;
					_unitOfWork.Book.Update(bookModel);
					await _unitOfWork.SaveAsync();
					#endregion

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
				Mapper.Map(bookModel, bookViewModel);

				List<CategoryViewModel> categories = (await _unitOfWork.Category.GetAllOrderedByDisplayOrderAsync())
					.Select(x => new CategoryViewModel
					{
						Id = x.Id,
						Name = x.Name
					}).ToList();
				bookViewModel.Categories = categories;

				List<TbBookImage> bookImages = _unitOfWork.
					BookImage
					.FindAllQueryable(x => x.BookId == id)
					.Select(x => new TbBookImage()
					{
						Id = x.Id,
						ImageUrl = x.ImageUrl,
					}).ToList();
				bookViewModel.BookImages = bookImages;

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
		public async Task<IActionResult> Edit(int id, AddEditBookViewModel bookViewModel, IFormFile? mainImage, List<IFormFile>? files)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var bookModel = await _unitOfWork.Book.GetByIdAsync(id);

					if (bookModel == null)
						return NotFound();

					// save book images
					if(mainImage != null)
					{
						// if the main image chaneged
						// remove the old main image from the database and from the hard desk as well
						var oldImageFromDb = await _unitOfWork.BookImage.GetAsync(x => x.BookId == id && x.IsMainImage);
						if (oldImageFromDb != null)
						{
							//// Delete old image
							string wwwRootPath = _webHostEnvironment.WebRootPath;

							string fullPath = Path.Combine(wwwRootPath, oldImageFromDb.ImageUrl.Trim('\\'));

							if (System.IO.File.Exists(fullPath))
							{
								System.IO.File.Delete(fullPath);
							}
							_unitOfWork.BookImage.Remove(oldImageFromDb);
						}
					}

					await _HandleBookImages(id, bookViewModel, mainImage, files);

					Mapper.Map(bookViewModel, bookModel);

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

				BookDetailsForAdminViewModel bookDetailsViewModel = new();
				Mapper.Map(bookModel, bookDetailsViewModel);

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

				BookDetailsForAdminViewModel bookDetailsViewModel = new();
				Mapper.Map(bookModel, bookDetailsViewModel);

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

		public async Task<IActionResult> DeleteImage(int imageId)
		{
			try
			{
				var bookImageFromDb = await _unitOfWork.BookImage.GetAsync(x => x.Id == imageId);

				if (bookImageFromDb == null)
					return NotFound();

				var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, bookImageFromDb.ImageUrl.Trim('\\'));

				if (System.IO.File.Exists(oldImagePath))
					System.IO.File.Delete(oldImagePath);

				_unitOfWork.BookImage.Remove(bookImageFromDb);
				await _unitOfWork.SaveAsync();

				TempData["success"] = "Book image deleted successfully!";
				return RedirectToAction(nameof(Edit), new { id = bookImageFromDb.BookId });
			}
			catch (Exception ex)
			{
				// Log exception (ex) here
				TempData["error"] = "An error occurred while retrieving the book for deletion.";
				return View("Error");
			}
		}

		private async Task _HandleBookImages(int bookId, AddEditBookViewModel bookViewModel, IFormFile? mainImage, List<IFormFile>? files)
		{
			try
			{
				string wwwRootPath = _webHostEnvironment.WebRootPath;
				string bookPath = @"uploads\images\books\book-" + bookId;
				string finalPath = Path.Combine(wwwRootPath, bookPath);

				if (!Directory.Exists(finalPath))
					Directory.CreateDirectory(finalPath);

				// save the main image
				if (mainImage != null)
				{
					await _CopayImage(mainImage, true);
				}

				// save others book images
				if (files != null)
				{
					foreach (var file in files)
					{
						await _CopayImage(file);
					}
				}

				async Task _CopayImage(IFormFile file, bool isMainImage = false)
				{
					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
					using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
					{
						await file.CopyToAsync(fileStream);
					}

					TbBookImage bookImage = new()
					{
						ImageUrl = @"\" + bookPath + @"\" + fileName,
						BookId = bookId,
						IsMainImage = isMainImage
					};

					bookViewModel.BookImages.Add(bookImage);
				}
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
