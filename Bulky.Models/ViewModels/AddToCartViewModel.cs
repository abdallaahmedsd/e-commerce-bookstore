
using System.ComponentModel.DataAnnotations;

namespace Bulky.Models.ViewModels
{
	public class AddToCartViewModel
	{
		public int BookId { get; set; }

		[Range(1, 1000)]
		public int Quantity { get; set; }
	}
}
