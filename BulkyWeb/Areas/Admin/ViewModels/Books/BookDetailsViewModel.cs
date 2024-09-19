using System.ComponentModel;

namespace BulkyWeb.Areas.Admin.ViewModels.Books
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
        public double ListPrice { get; set; }

        public double Price { get; set; }

        [DisplayName("Price for +50 units")]
        public double Price50 { get; set; }

        [DisplayName("Price for +100 units")]
        public double Price100 { get; set; }
    }
}