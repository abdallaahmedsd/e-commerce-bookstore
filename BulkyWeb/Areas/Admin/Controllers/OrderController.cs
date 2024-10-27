using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Orders;
using Bulky.Models.ViewModels.Admin;
using Bulky.Utility;
using BulkyWeb.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area(SD.Role_Admin)]
	[Authorize]
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
			try
			{
				OrderViewModel = new()
				{
					Order = await _unitOfWork.Order.GetByIdAsync(orderId, includeProperties: "User"),
					OrderDetails = await _unitOfWork.OrderDetail.FindAllAsync(x => x.OrderId == orderId, includeProperties: "Book")
				};

				return View(OrderViewModel);
			}
			catch (Exception ex)
			{
				TempData["Error"] = "An error occurred while loading order details. Please try again.";
				return RedirectToAction(nameof(Index));
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public async Task<IActionResult> UpdateOrderDetails()
		{
			try
			{
				TbOrder? orderFromDb = await _unitOfWork.Order.GetByIdAsync(OrderViewModel.Order.Id);

				Mapper.Map(OrderViewModel, orderFromDb);

				_unitOfWork.Order.Update(orderFromDb);
				await _unitOfWork.SaveAsync();

				TempData["Success"] = "Order details updated successfully!";
				return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.Order.Id });
			}
			catch (Exception ex)
			{
				TempData["Error"] = "An error occurred while updating order details. Please try again.";
				return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.Order.Id });
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public async Task<IActionResult> StartProccessing()
		{
			try
			{
				_unitOfWork.Order.UpdateStatus(OrderViewModel.Order.Id, SD.StatusInProcess);
				await _unitOfWork.SaveAsync();

				TempData["Success"] = "Order status updated successfully!";
				return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.Order.Id });
			}
			catch (Exception ex)
			{
				TempData["Error"] = "An error occurred while starting order processing. Please try again.";
				return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.Order.Id });
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public async Task<IActionResult> ShipOrder()
		{
			try
			{
				var orderFromDb = await _unitOfWork.Order.GetByIdAsync(OrderViewModel.Order.Id);
				orderFromDb.TrackingNumber = OrderViewModel.Order.TrackingNumber;
				orderFromDb.Carrier = OrderViewModel.Order.Carrier;
				orderFromDb.OrderStatus = SD.StatusShipped;
				orderFromDb.ShippingDate = DateTime.Now;

				if (orderFromDb.PaymentStatus == SD.PaymentStatusDelayedPayment)
				{
					orderFromDb.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
				}

				_unitOfWork.Order.Update(orderFromDb);
				await _unitOfWork.SaveAsync();

				TempData["Success"] = "Order shipped successfully!";
				return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.Order.Id });
			}
			catch (Exception ex)
			{
				TempData["Error"] = "An error occurred while shipping the order. Please try again.";
				return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.Order.Id });
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
		public async Task<IActionResult> CancelOrder()
		{
			try
			{
				var orderFromDb = await _unitOfWork.Order.GetByIdAsync(OrderViewModel.Order.Id);

				if (orderFromDb.PaymentStatus == SD.PaymentStatusApproved)
				{
					var options = new RefundCreateOptions
					{
						Reason = RefundReasons.RequestedByCustomer,
						PaymentIntent = orderFromDb.PaymentIntentId,
					};

					var service = new RefundService();
					Refund refund = await service.CreateAsync(options);

					_unitOfWork.Order.UpdateStatus(orderFromDb.Id, SD.StatusCanceled, SD.StatusRefunded);
				}
				else
				{
					_unitOfWork.Order.UpdateStatus(orderFromDb.Id, SD.StatusCanceled, SD.StatusCanceled);
				}

				await _unitOfWork.SaveAsync();
				TempData["Success"] = "Order canceled successfully!";
				return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.Order.Id });
			}
			catch (Exception ex)
			{
				TempData["Error"] = "An error occurred while canceling the order. Please try again.";
				return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.Order.Id });
			}
		}

		[ActionName("Details")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Details_PAY_NOW()
		{
			try
			{
				OrderViewModel.Order = await _unitOfWork.Order
					.GetAsync(u => u.Id == OrderViewModel.Order.Id, includeProperties: "User");

				OrderViewModel.OrderDetails = await _unitOfWork.OrderDetail
					.FindAllAsync(u => u.OrderId == OrderViewModel.Order.Id, includeProperties: "Book");

				var domain = Request.Scheme + "://" + Request.Host.Value + "/";
				var options = new SessionCreateOptions
				{
					SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderId={OrderViewModel.Order.Id}",
					CancelUrl = domain + $"admin/order/details?orderId={OrderViewModel.Order.Id}",
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",
				};

				foreach (var item in OrderViewModel.OrderDetails)
				{
					var sessionLineItem = new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmount = (long)(item.Price * 100),
							Currency = "usd",
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = item.Book.Title
							}
						},
						Quantity = item.Quantity,
					};

					options.LineItems.Add(sessionLineItem);
				}

				var service = new SessionService();
				Session session = service.Create(options);

				_unitOfWork.Order.UpdateStripePaymentId(OrderViewModel.Order.Id, session.Id, session.PaymentIntentId);
				await _unitOfWork.SaveAsync();
				Response.Headers.Add("Location", session.Url);
				return new StatusCodeResult(303);
			}
			catch (Exception ex)
			{
				TempData["Error"] = "An error occurred while processing payment. Please try again.";
				return RedirectToAction(nameof(Details), new { orderId = OrderViewModel.Order.Id });
			}
		}

		public async Task<IActionResult> PaymentConfirmation(int orderId)
		{
			try
			{
				TbOrder order = await _unitOfWork.Order.GetAsync(u => u.Id == orderId);

				if (order.PaymentStatus == SD.PaymentStatusDelayedPayment)
				{
					var service = new SessionService();
					Session session = service.Get(order.SessionId);

					if (session.PaymentStatus.ToLower() == "paid")
					{
						_unitOfWork.Order.UpdateStripePaymentId(orderId, session.Id, session.PaymentIntentId);
						_unitOfWork.Order.UpdateStatus(orderId, order.OrderStatus, SD.PaymentStatusApproved);
						await _unitOfWork.SaveAsync();
					}
				}

				return View(orderId);
			}
			catch (Exception ex)
			{
				TempData["Error"] = "An error occurred while confirming payment. Please try again.";
				return RedirectToAction(nameof(Details), new { orderId });
			}
		}
	}
}
