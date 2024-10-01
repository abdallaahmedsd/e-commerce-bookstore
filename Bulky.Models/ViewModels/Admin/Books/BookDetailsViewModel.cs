using System.ComponentModel;

namespace Bulky.Models.ViewModels.Admin.Books
{
	public class BookDetailsViewModel
    {
        public int Id { get; set; }

        [DisplayName("Book Title")]
        public string Title { get; set; } = null!;

        [DisplayName("Book Description")]
        public string Description { get; set; } = null!;

        public string ISBN { get; set; } = null!;

        [DisplayName("Author Name")]
        public string Author { get; set; } = null!;

        [DisplayName("List Price")]
        public decimal ListPrice { get; set; }

        public decimal Price { get; set; }

        [DisplayName("Price for +50 units")]
        public decimal Price50 { get; set; }

        [DisplayName("Price for +100 units")]
        public decimal Price100 { get; set; }

        [DisplayName("Book Image")]
        public string ImageUrl { get; set; } = null!;

		public string Category { get; set; } = null!;
	}
}