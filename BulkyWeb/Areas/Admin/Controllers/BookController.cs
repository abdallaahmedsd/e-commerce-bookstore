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
		public async Task<IActionResult> Create(AddEditBookViewModel bookViewModel, List<IFormFile> files)
		{
			if (ModelState.IsValid)
			{
				try
				{
					TbBook bookModel = new();
					Mapper.Map(bookViewModel, bookModel);

					await _unitOfWork.Book.AddAsync(bookModel);

					await _unitOfWork.SaveAsync();

					// save book images
					string wwwRootPath = _webHostEnvironment.WebRootPath;
					foreach(var file in files)
					{
						string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
						string bookPath = @"uploads\images\books\book-" + bookModel.Id; 
						string finalPath = Path.Combine(wwwRootPath, bookPath);

						if(!Directory.Exists(finalPath))
							Directory.CreateDirectory(finalPath);

						using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
						{
							await file.CopyToAsync(fileStream);
						}

						TbBookImage bookImage = new()
						{
							ImageUrl = @"\" + bookPath + @"\" + fileName,
							BookId = bookModel.Id,
						};

						bookViewModel.BookImages.Add(bookImage);
					}
					// save the new images
					bookModel.BookImages = bookViewModel.BookImages;
					_unitOfWork.Book.Update(bookModel);
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
		public async Task<IActionResult> Edit(int id, AddEditBookViewModel bookViewModel, List<IFormFile>? files)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var bookModel = await _unitOfWork.Book.GetByIdAsync(id);

					if (bookModel == null)
						return NotFound();

					/*if (file != null)
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
					}*/

					// save book images
					if(files != null)
					{
						string wwwRootPath = _webHostEnvironment.WebRootPath;
						foreach (var file in files)
						{
							string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
							string bookPath = @"uploads\images\books\book-" + bookModel.Id;
							string finalPath = Path.Combine(wwwRootPath, bookPath);

							if (!Directory.Exists(finalPath))
								Directory.CreateDirectory(finalPath);

							using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
							{
								await file.CopyToAsync(fileStream);
							}

							TbBookImage bookImage = new()
							{
								ImageUrl = @"\" + bookPath + @"\" + fileName,
								BookId = bookModel.Id,
							};

							bookViewModel.BookImages.Add(bookImage);
						}
					}

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

				if(System.IO.File.Exists(oldImagePath))
					System.IO.File.Delete(oldImagePath);

				_unitOfWork.BookImage.Remove(bookImageFromDb);
				await _unitOfWork.SaveAsync();

				TempData["success"] = "Book image deleted successfully!";
				return RedirectToAction(nameof(Edit), new {id = bookImageFromDb.BookId});
            }
            catch (Exception ex)
            {
                // Log exception (ex) here
                TempData["error"] = "An error occurred while retrieving the book for deletion.";
                return View("Error");
            }
        }
    }
}
