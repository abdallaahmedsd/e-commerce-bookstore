using Bulky.Models.Orders;

namespace Bulky.Models.ViewModels.Admin
{
	public class OrderViewModel
	{
		public TbOrder Order { get; set; }
		public IEnumerable<TbOrderDetail> OrderDetails { get; set; } = [];
	}
}
