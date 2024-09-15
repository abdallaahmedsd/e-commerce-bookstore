using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Category Category { get; set; }

        public IActionResult OnGet(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            Category = _context.Categories.Find(id);

            if (Category == null)
                return NotFound();

            return Page();
        }

        public IActionResult OnPost(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            Category = _context.Categories.Find(id);

            if (Category == null)
                return NotFound();

            _context.Categories.Remove(Category);
            _context.SaveChanges();

            TempData["success"] = "Category deleted successfully!";
            return RedirectToPage("Index");
        }
    }
}
