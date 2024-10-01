using System.ComponentModel.DataAnnotations;
using Bulky.Models.ViewModels.Admin.Books;

namespace Bulky.Models.ViewModels
{
    public class ShoppingCartViewModel
    {
        public BookDetailsViewModel BookDetails { get; set; } = null!;

        [Range(1, 1000)]
        public int Quantity { get; set; }
    }
}