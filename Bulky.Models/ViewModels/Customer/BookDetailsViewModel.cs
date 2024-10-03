using System.ComponentModel.DataAnnotations;
using Bulky.Models.ViewModels.Admin.Books;

namespace Bulky.Models.ViewModels.Customer
{
	public class BookDetailsViewModel
    {
        public BookDetailsForAdminViewModel BookDetails { get; set; } = null!;

        [Range(1, 1000)]
        public int Quantity { get; set; }
    }
}