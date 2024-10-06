using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels.Customer;
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

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
		{
            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User?.Identity;
            int userId = int.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);

            var shoppingCarts = await _unitOfWork.ShoppingCart.FindAllAsync(x => x.UserId == userId, "Book");

            var shoppingCartsViewModel = new ShoppingCartViewModel 
            {
                LstShoppingCarts = shoppingCarts,
                Order = new()
            };

            _CalcOrderTotal(shoppingCartsViewModel);

			return View(shoppingCartsViewModel);
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

			var shoppingCartsViewModel = new ShoppingCartViewModel
			{
				LstShoppingCarts = shoppingCarts,
				Order = new()
			};

			shoppingCartsViewModel.Order.User = await _unitOfWork.ApplicationUser?.GetByIdAsync(userId);

            Mapper.Map(shoppingCartsViewModel.Order.User, shoppingCartsViewModel.Order);

			_CalcOrderTotal(shoppingCartsViewModel);

			return View(shoppingCartsViewModel);
        }

		private decimal _GetPriceBasedOnQuantity(TbShoppingCart cart)
        {
            if(cart.Quantity <= 50) 
            {
                return cart.Book.Price;
            }

            if(cart.Quantity <= 100)
            {
                return cart.Book.Price50;
            }

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
