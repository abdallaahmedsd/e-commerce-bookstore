namespace Bulky.Models.ViewModels
{
    public class BookListViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Author { get; set; } = null!;

        public string ISBN { get; set; } = null!;

        public decimal Price { get; set; }

        public string Category { get; set; } = null!;
    }
}
