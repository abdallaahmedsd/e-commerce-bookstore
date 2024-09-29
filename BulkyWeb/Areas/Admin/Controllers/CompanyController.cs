using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels.Admin;
using Bulky.Utility;
using BulkyWeb.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area(SD.Role_Admin)]
    public class CompanyController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public CompanyController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Create()
		{
			return View(new CompanyViewModel());
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyViewModel companyViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var companyModel = new TbCompany();

                    Mapper.Map(companyViewModel, companyModel);

                    await _unitOfWork.Company.AddAsync(companyModel);

                    await _unitOfWork.SaveAsync();
                    TempData["success"] = "Company created successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Log exception (ex) here
                    TempData["error"] = "An error occurred while creating the company.";
                    return View("Error");
                }
            }

            return View(companyViewModel);
        }
    }
}
