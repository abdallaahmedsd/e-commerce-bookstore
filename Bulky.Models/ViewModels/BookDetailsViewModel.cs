using System.ComponentModel.DataAnnotations;
using Bulky.Models.ViewModels.Admin.Books;

namespace Bulky.Models.ViewModels
{
    public class BookDetailsViewModel
    {
        public BookDetailsForAdminViewModel BookDetails { get; set; } = null!;

        [Range(1, 1000)]
        public int Quantity { get; set; }
    }

    public class ShoppingCartViewModel
    {
        public IEnumerable<TbShoppingCart> LstShoppingCarts { get; set; } = [];

        public decimal OrderTotal { get; set; }
    }
}