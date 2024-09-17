using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var lstCategories = await _unitOfWork.Category.GetAllAsync();
                return View(lstCategories);
            }
            catch (Exception ex)
            {
                // Log exception (ex) here
                TempData["error"] = "An error occurred while retrieving categories.";
                return View("Error");
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _unitOfWork.Category.AddAsync(category);
                    await _unitOfWork.SaveAsync();
                    TempData["success"] = "Category created successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Log exception (ex) here
                    TempData["error"] = "An error occurred while creating the category.";
                    return View("Error");
                }
            }

            return View(category);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
                return NotFound();

            try
            {
                var category = await _unitOfWork.Category.GetByIdAsync(id);

                if (category == null)
                    return NotFound();

                return View(category);
            }
            catch (Exception ex)
            {
                // Log exception (ex) here
                TempData["error"] = "An error occurred while retrieving the category for editing.";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Category updateCategory)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var oldCategory = await _unitOfWork.Category.GetByIdAsync(id);

                    if (oldCategory == null)
                        return NotFound();

                    oldCategory.Name = updateCategory.Name;
                    oldCategory.DisplayOrder = updateCategory.DisplayOrder;

                    _unitOfWork.Category.Update(oldCategory);
                    await _unitOfWork.SaveAsync();
                    TempData["success"] = "Category updated successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Log exception (ex) here
                    TempData["error"] = "An error occurred while updating the category.";
                    return View("Error");
                }
            }

            return View(updateCategory);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return NotFound();

            try
            {
                var category = await _unitOfWork.Category.GetByIdAsync(id);

                if (category == null)
                    return NotFound();

                return View(category);
            }
            catch (Exception ex)
            {
                // Log exception (ex) here
                TempData["error"] = "An error occurred while retrieving the category for deletion.";
                return View("Error");
            }
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var category = await _unitOfWork.Category.GetByIdAsync(id);

                    if (category == null)
                        return NotFound();

                    _unitOfWork.Category.Remove(category);
                    await _unitOfWork.SaveAsync();
                    TempData["success"] = "Category deleted successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // Log exception (ex) here
                    TempData["error"] = "An error occurred while deleting the category.";
                    return View("Error");
                }
            }

            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return NotFound();

            try
            {
                var category = await _unitOfWork.Category.GetByIdAsync(id);

                if (category == null)
                    return NotFound();

                return View(category);
            }
            catch (Exception ex)
            {
                // Log exception (ex) here
                TempData["error"] = "An error occurred while retrieving category details.";

                /* ErrorViewModel model = new ErrorViewModel() { RequestId = Guid.NewGuid().ToString() };
                 return View("Error", model);*/

                return View("Error");
            }
        }
    }
}
