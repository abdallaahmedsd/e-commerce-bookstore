using Bulky.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Bulky.Utility;
using Bulky.Models.Orders;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BulkyWeb.Areas.Admin.ApiControllers
{
	[Route("api/admin/[controller]")]
	[ApiController]
	[Authorize]
	public class OrdersController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
		public IActionResult GetAll(string status = "all")
		{
			try
			{
				IQueryable<TbOrder> lstOrders;
				if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
				{
					lstOrders = _unitOfWork.Order.GetAllQueryable(includeProperties: "User");
				}
				else
				{
					ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
					int userId = int.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);

					lstOrders = _unitOfWork.Order.FindAllQueryable(x => x.UserId == userId ,includeProperties: "User");
				}

				switch (status)
				{
					case "pending":
						lstOrders = lstOrders.Where(x => x.PaymentStatus == SD.PaymentStatusDelayedPayment);
						break;
					case "inProcess":
						lstOrders = lstOrders.Where(x => x.OrderStatus == SD.StatusInProcess);
						break;
					case "completed":
						lstOrders = lstOrders.Where(x => x.OrderStatus == SD.StatusShipped);
						break;
					case "approved":
						lstOrders = lstOrders.Where(x => x.OrderStatus == SD.StatusApproved);
						break;
					default:
						break;
				}

				return Ok(new { success = true, data = lstOrders });
			}
			catch (Exception ex)
			{
				// Log the exception details (optional)
				return StatusCode(500, new { success = false, message = "An error occurred while retrieving orders." });
			}
		}
	}
}
