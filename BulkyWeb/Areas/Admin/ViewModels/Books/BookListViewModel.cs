namespace BulkyWeb.Areas.Admin.ViewModels.Books
{
    public class BookListViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Author { get; set; } = null!;

        public string ISBN { get; set; } = null!;

        public double ListPrice { get; set; }

        public string? Category { get; set; }
    }
}
