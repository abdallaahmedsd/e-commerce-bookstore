using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Models.ViewModels.Admin.Books;
using Bulky.Models.ViewModels.Customer;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IReadOnlyRepository<BookHomeViewModel> _readOnlyRepository;

		public HomeController(IUnitOfWork unitOfWork, IReadOnlyRepository<BookHomeViewModel> readOnlyRepository, ILogger<HomeController> logger)
        {
			_unitOfWork = unitOfWork;
			_readOnlyRepository = readOnlyRepository;
			_logger = logger;
        }

		public async Task<IActionResult> Index()
		{
			try
			{
				var lstBooks = await _readOnlyRepository.GetAllAsync();

				// order them randomly 
				lstBooks = [.. lstBooks.OrderBy(x => Guid.NewGuid())];
				return View(lstBooks);
			}
			catch (Exception ex)
			{
				// Log exception (ex) here
				TempData["error"] = "An error occurred while retrieving the books.";
				return View("Error");
			}
		}

		public IActionResult Privacy()
        {
            return View();
        }

		public async Task<IActionResult> Details(int bookId)
		{
			if (bookId <= 0)
				return NotFound();

			try
			{
				var bookModel = await _unitOfWork.Book.GetByIdAsync(bookId, "Category");

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

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
