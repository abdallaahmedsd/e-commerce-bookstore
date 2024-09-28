using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Company.Controllers
{
	public class CompanyController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
