using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.Identity;
using Bulky.Models.Orders;
using Bulky.Models.ViewModels.Customer;
using Bulky.Utility;
using BulkyWeb.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class CartController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartViewModel { get; set; } 

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
		{
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User?.Identity;
            int userId = int.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);

            var shoppingCarts = await _unitOfWork.ShoppingCart.FindAllAsync(x => x.UserId == userId, "Book");

            ShoppingCartViewModel = new ShoppingCartViewModel 
            {
                LstShoppingCarts = shoppingCarts,
                Order = new()
            };

            _CalcOrderTotal(ShoppingCartViewModel);

			return View(ShoppingCartViewModel);
		}

        public async Task<IActionResult> IncrementQuantity(int cartId) 
        {
			if (cartId <= 0)
				return BadRequest();

			var cartFromDb = await _unitOfWork.ShoppingCart.GetAsync(x => x.Id == cartId);

            if(cartFromDb == null)
                return NotFound();

            cartFromDb.Quantity++;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

		public async Task<IActionResult> DecrementQuantity(int cartId)
		{
			if (cartId <= 0)
				return BadRequest();

			var cartFromDb = await _unitOfWork.ShoppingCart.GetAsync(x => x.Id == cartId);

			if (cartFromDb == null)
				return NotFound();

            if(cartFromDb.Quantity <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
            }
            else
            {
				cartFromDb.Quantity--;
				_unitOfWork.ShoppingCart.Update(cartFromDb);
			}

			await _unitOfWork.SaveAsync();

			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Delete(int cartId)
		{
			if (cartId <= 0)
				return BadRequest();

			var cartFromDb = await _unitOfWork.ShoppingCart.GetAsync(x => x.Id == cartId);

			if (cartFromDb == null)
				return NotFound();

			_unitOfWork.ShoppingCart.Remove(cartFromDb);
			await _unitOfWork.SaveAsync();

			return RedirectToAction(nameof(Index));
		}

        public async Task<IActionResult> Summary()
        {
			ClaimsIdentity claimsIdentity = (ClaimsIdentity)User?.Identity;
			int userId = int.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);

			var shoppingCarts = await _unitOfWork.ShoppingCart.FindAllAsync(x => x.UserId == userId, "Book");

			ShoppingCartViewModel = new ShoppingCartViewModel
			{
				LstShoppingCarts = shoppingCarts,
				Order = new()
			};

			ShoppingCartViewModel.Order.User = await _unitOfWork.ApplicationUser?.GetByIdAsync(userId);

            Mapper.Map(ShoppingCartViewModel.Order.User, ShoppingCartViewModel.Order);

			_CalcOrderTotal(ShoppingCartViewModel);

			return View(ShoppingCartViewModel);
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName("Summary")]
		public async Task<IActionResult> PlaceOrder()
		{
			ClaimsIdentity claimsIdentity = (ClaimsIdentity)User?.Identity;
			int userId = int.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);

			var shoppingCarts = await _unitOfWork.ShoppingCart.FindAllAsync(x => x.UserId == userId, "Book");

			ShoppingCartViewModel.LstShoppingCarts = shoppingCarts;

			ShoppingCartViewModel.Order.OrderDate = DateTime.Now;
			ShoppingCartViewModel.Order.UserId = userId;

			_CalcOrderTotal(ShoppingCartViewModel);

			ApplicationUser? user = await _unitOfWork.ApplicationUser.GetByIdAsync(userId);

			_SetOrderStatus(ShoppingCartViewModel, user);

			// create the order 
			await _unitOfWork.Order.AddAsync(ShoppingCartViewModel.Order);
			await _unitOfWork.SaveAsync();

			// save order details
			foreach(var item in  ShoppingCartViewModel.LstShoppingCarts)
			{
				TbOrderDetail detail = new()
				{
					OrderId = ShoppingCartViewModel.Order.Id,
					BookId = item.BookId,
					Price = item.Price,
					Quantity = item.Quantity,
				};

				await _unitOfWork.OrderDetail.AddAsync(detail);
			}

			await _unitOfWork.SaveAsync();

			return RedirectToAction(nameof(OrderConfirmation), new { orderId = ShoppingCartViewModel.Order.Id });
		}

		public IActionResult OrderConfirmation(int orderId)
		{
			return View(orderId);
		}

		private void _SetOrderStatus(ShoppingCartViewModel shoppingCartViewModel, ApplicationUser user)
		{
			// check if the user has a company or not
			if (user.CompanyId.GetValueOrDefault() == 0) 
			{
				// incase of a regualr customer
				shoppingCartViewModel.Order.PaymentStatus = SD.PaymentStatusPending;
				shoppingCartViewModel.Order.OrderStatus = SD.StatusPending;
			}
			else
			{
				// incase of a user has a company
				shoppingCartViewModel.Order.PaymentStatus = SD.PaymentStatusDelayedPayment;
				shoppingCartViewModel.Order.OrderStatus = SD.StatusApproved;
			}
		}

		private decimal _GetPriceBasedOnQuantity(TbShoppingCart cart)
        {
            if(cart.Quantity <= 50) 
                return cart.Book.Price;

            if(cart.Quantity <= 100)
                return cart.Book.Price50;

            return cart.Book.Price100;
        }

        private void _CalcOrderTotal(ShoppingCartViewModel cartVM)
        {
            foreach(var cart in cartVM.LstShoppingCarts)
            {
                cart.Price = _GetPriceBasedOnQuantity(cart);

				cartVM.Order.OrderTotal += (cart.Price * cart.Quantity);
            }
        }
	}
}
