namespace Bulky.Models.ViewModels.Customer
{
	public class ShoppingCartViewModel
    {
        public IEnumerable<TbShoppingCart> LstShoppingCarts { get; set; } = [];

        public decimal OrderTotal { get; set; }
    }
}