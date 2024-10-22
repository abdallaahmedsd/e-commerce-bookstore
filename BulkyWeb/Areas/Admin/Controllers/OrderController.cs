using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Orders;
using Bulky.Models.ViewModels.Admin;
using Bulky.Utility;
using BulkyWeb.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area(SD.Role_Admin)]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		[BindProperty]
		public OrderViewModel OrderViewModel { get; set; }

		public OrderController(IUnitOfWork unitOfWork) 
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> Details(int orderId)
		{
			OrderViewModel = new()
			{
				Order = await _unitOfWork.Order.GetByIdAsync(orderId, includeProperties: "User"),
				OrderDetails = await _unitOfWork.OrderDetail.FindAllAsync(x => x.OrderId == orderId, includeProperties: "Book")
			};

			return View(OrderViewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public async Task<IActionResult> UpdateOrderDetails() 
		{
			TbOrder? orderFromDb = await _unitOfWork.Order.GetByIdAsync(OrderViewModel.Order.Id);

			Mapper.Map(OrderViewModel, orderFromDb);

			_unitOfWork.Order.Update(orderFromDb);
			await _unitOfWork.SaveAsync();

			TempData["Success"] = "Order details updated successfully!";

			return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.Order.Id });
		}
	}
}
