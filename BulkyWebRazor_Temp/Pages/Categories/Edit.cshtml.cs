using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    public class EditModel : PageModel
    {
		private readonly AppDbContext _context;

		public EditModel(AppDbContext context)
		{
			_context = context;
		}

		[BindProperty]
		public Category Category { get; set; }

		public IActionResult OnGet(int? id)
		{
			if(id == null || id == 0)
				return NotFound();

			Category = _context.Categories.Find(id);

			if(Category == null)
				return NotFound();

            return Page();
		}

		public IActionResult OnPost(int? id)
		{
			if (Category != null)
			{
				if (id == null || id == 0)
					return NotFound();

				var oldCategory = _context.Categories.Find(id);

				if (oldCategory == null)
					return NotFound();

				oldCategory.Name = Category.Name;
				oldCategory.DisplayOrder = Category.DisplayOrder;

				_context.Categories.Update(oldCategory);
				_context.SaveChanges();

                TempData["success"] = "Category updated successfully!";
                return RedirectToPage("Index");
			}

			return Page();
		}
	}
}
