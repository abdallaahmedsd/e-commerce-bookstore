using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BulkyWeb.Areas.Admin.ViewModels.Books
{
    public class AddEditBookViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters")]
        [DisplayName("Book Title")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Description is required")]
        [DisplayName("Book Description")]
        public string Description { get; set; } = null!;

        [Required(ErrorMessage = "ISBN is required")]
        [StringLength(20, ErrorMessage = "ISBN cannot be longer than 20 characters")]
        public string ISBN { get; set; } = null!;

        [Required(ErrorMessage = "Author is required")]
        [StringLength(100, ErrorMessage = "Author name cannot be longer than 100 characters")]
        [DisplayName("Author Name")]
        public string Author { get; set; } = null!;

        [Range(0, double.MaxValue, ErrorMessage = "List Price must be a positive number")]
        [DisplayName("List Price")]
        public double ListPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number")]
        public double Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price for +50 units must be a positive number")]
        [DisplayName("Price for +50 units")]
        public double Price50 { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price for +100 units must be a positive number")]
        [DisplayName("Price for +100 units")]
        public double Price100 { get; set; }
    }
}