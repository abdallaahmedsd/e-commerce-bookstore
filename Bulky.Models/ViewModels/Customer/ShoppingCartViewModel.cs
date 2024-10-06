using Bulky.Models.Orders;

namespace Bulky.Models.ViewModels.Customer
{
	public class ShoppingCartViewModel
    {
        public IEnumerable<TbShoppingCart> LstShoppingCarts { get; set; } = [];

        public TbOrder Order { get; set; } = null!; 
    }
}