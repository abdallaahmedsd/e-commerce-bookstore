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

        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return BadRequest();

            try
            {
                var companyModel = await _unitOfWork.Company.GetByIdAsync(id);

                if (companyModel == null)
                    return NotFound();

                var companyViewModel = new CompanyViewModel();
                Mapper.Map(companyModel, companyViewModel);

                return View(companyViewModel);
            }
            catch (Exception ex)
            {
                // Log exception (ex) here
                TempData["error"] = "An error occurred while retrieving the company for editing.";
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CompanyViewModel updateCompany)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var oldCompany = await _unitOfWork.Company.GetByIdAsync(id);

                    if (oldCompany == null)
                        return NotFound();

                    Mapper.Map(updateCompany, oldCompany);

                    _unitOfWork.Company.Update(oldCompany);
                    await _unitOfWork.SaveAsync();
                    TempData["success"] = "Company updated successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Log exception (ex) here
                    TempData["error"] = "An error occurred while updating the company.";
                    return View("Error");
                }
            }

            return View(updateCompany);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return NotFound();

            try
            {
                var companyModel = await _unitOfWork.Company.GetByIdAsync(id);

                if (companyModel == null)
                    return NotFound();

                var companyViewModel = new CompanyViewModel();
                Mapper.Map(companyModel, companyViewModel);

                return View(companyViewModel);
            }
            catch (Exception ex)
            {
                // Log exception (ex) here
                TempData["error"] = "An error occurred while retrieving the company for deletion.";
                return View("Error");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return NotFound();

            try
            {
                var companyModel = await _unitOfWork.Company.GetByIdAsync(id);

                if (companyModel == null)
                    return NotFound();

                var companyViewModel = new CompanyViewModel();
                Mapper.Map(companyModel, companyViewModel);

                return View(companyViewModel);
            }
            catch (Exception ex)
            {
                // Log exception (ex) here
                TempData["error"] = "An error occurred while retrieving company details.";

                /* ErrorViewModel model = new ErrorViewModel() { RequestId = Guid.NewGuid().ToString() };
                 return View("Error", model);*/

                return View("Error");
            }
        }
    }
}
