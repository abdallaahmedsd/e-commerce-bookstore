using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels.Admin;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BulkyWeb.Mappers;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area(SD.Role_Admin)]
    //[Authorize(Roles = SD.Role_Admin)]
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
				var lstCategoriesModels = await _unitOfWork.Category.GetAllOrderedByDisplayOrderAsync();

				var lstCategoriesViewModels = new List<CategoryViewModel>();

				foreach (var category in lstCategoriesModels)
				{
					lstCategoriesViewModels.Add(new CategoryViewModel
					{
						Id = category.Id,
						Name = category.Name,
						DisplayOrder = category.DisplayOrder
					});
				}

				return View(lstCategoriesViewModels);
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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CategoryViewModel categoryViewModel)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _unitOfWork.Category.AddAsync(new TbCategory
					{
						Name = categoryViewModel.Name,
						DisplayOrder = categoryViewModel.DisplayOrder
					});

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

			return View(categoryViewModel);
		}

		public async Task<IActionResult> Edit(int id)
		{
			if (id <= 0)
				return NotFound();

			try
			{
				var categoryModel = await _unitOfWork.Category.GetByIdAsync(id);

				if (categoryModel == null)
					return NotFound();

				var categoryViewModel = new CategoryViewModel();
				Mapper.Map(categoryModel, categoryViewModel);

				return View(categoryViewModel);
			}
			catch (Exception ex)
			{
				// Log exception (ex) here
				TempData["error"] = "An error occurred while retrieving the category for editing.";
				return View("Error");
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, CategoryViewModel updateCategory)
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
				var categoryModel = await _unitOfWork.Category.GetByIdAsync(id);

				if (categoryModel == null)
					return NotFound();

				var categoryViewModel = new CategoryViewModel();
				Mapper.Map(categoryModel, categoryViewModel);

				return View(categoryViewModel);
			}
			catch (Exception ex)
			{
				// Log exception (ex) here
				TempData["error"] = "An error occurred while retrieving the category for deletion.";
				return View("Error");
			}
		}

		public async Task<IActionResult> Details(int id)
		{
			if (id <= 0)
				return NotFound();

			try
			{
				var categoryModel = await _unitOfWork.Category.GetByIdAsync(id);

				if (categoryModel == null)
					return NotFound();

				var categoryViewModel = new CategoryViewModel();
				Mapper.Map(categoryModel, categoryViewModel);

				return View(categoryViewModel);
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
