using Bulky.DataAccess.Data;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var lstCategories = _context.Categories.ToList();
            return View(lstCategories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
		public IActionResult Create(Category category)
		{
            if(ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
				TempData["success"] = "Category created successfully!";
				return RedirectToAction("Index");
            }

			return View(category);
		}

		public IActionResult Edit(int? id)
		{
            if(id == null || id <= 0)
                return NotFound();

            var category = _context.Categories.Find(id);

            if(category == null)
                return NotFound();

			return View(category);
		}

		[HttpPost]
		public IActionResult Edit(int id, Category updateCategory)
		{
			if (ModelState.IsValid)
			{
				var oldCategory = _context.Categories.Find(id);

                if (oldCategory == null)
                    return NotFound();

				oldCategory.Name = updateCategory.Name;
                oldCategory.DisplayOrder = updateCategory.DisplayOrder;
				_context.SaveChanges();
				TempData["success"] = "Category updated successfully!";
				return RedirectToAction("Index");
			}

			return View(updateCategory);
		}

		public IActionResult Delete(int? id)
		{
			if (id == null || id <= 0)
				return NotFound();

			var category = _context.Categories.Find(id);

			if (category == null)
				return NotFound();

			return View(category);
		}

		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePost(int? id)
		{
			if (ModelState.IsValid)
			{
				var category = _context.Categories.Find(id);

				if (category == null)
					return NotFound();
				
				_context.Categories.Remove(category);
				_context.SaveChanges();
				TempData["success"] = "Category deleted successfully!";
				return RedirectToAction("Index");
			}

			return View();
		}

		public IActionResult Details(int? id)
		{
            if (id == null || id <= 0)
                return NotFound();

            var category = _context.Categories.Find(id);

            if (category == null)
                return NotFound();

            return View(category);
        }
	}
}
