namespace Bulky.Models
{
    public class TbBook
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string ISBN { get; set; } = null!;

        public string Author { get; set; } = null!;

        public double ListPrice { get; set; }

        public double Price { get; set; }

        public double Price50 { get; set; }

        public double Price100 { get; set; }

        public int CategoryId { get; set; }
        public TbCategory Category { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;
    }
}
