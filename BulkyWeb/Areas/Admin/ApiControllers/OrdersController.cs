using Bulky.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System;
using Bulky.Utility;

namespace BulkyWeb.Areas.Admin.ApiControllers
{
	[Route("api/admin/[controller]")]
	[ApiController]
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
				var lstOrders = _unitOfWork.Order.GetAllQueryable(includeProperties: "User");

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
