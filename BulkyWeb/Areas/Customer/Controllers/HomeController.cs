using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.ViewModels;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
