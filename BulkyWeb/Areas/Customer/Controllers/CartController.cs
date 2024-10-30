using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.Identity;
using Bulky.Models.Orders;
using Bulky.Models.ViewModels.Customer;
using Bulky.Utility;
using BulkyWeb.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
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
            try
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
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading the cart. Please try again.";
                // Log the exception (ex) if needed
                return View(new ShoppingCartViewModel()); // Return an empty view model
            }
        }

        public async Task<IActionResult> IncrementQuantity(int cartId)
        {
            try
            {
                if (cartId <= 0)
                    return BadRequest();

                var cartFromDb = await _unitOfWork.ShoppingCart.GetAsync(x => x.Id == cartId);

                if (cartFromDb == null)
                    return NotFound();

                cartFromDb.Quantity++;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                await _unitOfWork.SaveAsync();

                TempData["Success"] = "Quantity incremented successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while incrementing quantity. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> DecrementQuantity(int cartId)
        {
            try
            {
                if (cartId <= 0)
                    return BadRequest();

                var cartFromDb = await _unitOfWork.ShoppingCart.GetAsync(x => x.Id == cartId);

                if (cartFromDb == null)
                    return NotFound();

                bool isOneItem = cartFromDb.Quantity <= 1;

                if (isOneItem)
                {
                    _unitOfWork.ShoppingCart.Remove(cartFromDb);
                    TempData["Success"] = "Item removed from cart.";
                }
                else
                {
                    cartFromDb.Quantity--;
                    _unitOfWork.ShoppingCart.Update(cartFromDb);
                    TempData["Success"] = "Quantity decremented successfully.";
                }

                await _unitOfWork.SaveAsync();

                if (isOneItem)
                    _SaveCartQuantityInSession(cartFromDb.UserId);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while decrementing quantity. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Delete(int cartId)
        {
            try
            {
                if (cartId <= 0)
                    return BadRequest();

                var cartFromDb = await _unitOfWork.ShoppingCart.GetAsync(x => x.Id == cartId);

                if (cartFromDb == null)
                    return NotFound();

                _unitOfWork.ShoppingCart.Remove(cartFromDb);
                await _unitOfWork.SaveAsync();

                _SaveCartQuantityInSession(cartFromDb.UserId);

                TempData["Success"] = "Item deleted from cart successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while deleting item from cart. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Summary()
        {
            try
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
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading the summary. Please try again.";
                return View(new ShoppingCartViewModel()); // Return an empty view model
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> PlaceOrder()
        {
            try
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

                // create order details
                foreach (var item in ShoppingCartViewModel.LstShoppingCarts)
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

                // save order details
                await _unitOfWork.SaveAsync();

                // if it is a regular customer account we need to capture payment
                if (user.CompanyId.GetValueOrDefault() == 0)
                {
                    //stripe logic
                    var domain = "https://localhost:7250/";
                    var options = new SessionCreateOptions
                    {
                        SuccessUrl = domain + $"customer/cart/OrderConfirmation?orderId={ShoppingCartViewModel.Order.Id}",
                        CancelUrl = domain + "customer/cart/index",
                        LineItems = new List<SessionLineItemOptions>(),
                        Mode = "payment",
                    };

                    foreach (var item in ShoppingCartViewModel.LstShoppingCarts)
                    {
                        var sessionLineItem = new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(item.Price * 100), // Price in cents
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = item.Book.Title
                                }
                            },
                            Quantity = item.Quantity
                        };
                        options.LineItems.Add(sessionLineItem);
                    }

                    var service = new SessionService();
                    Session session = service.Create(options);

                    _unitOfWork.Order.UpdateStripePaymentId(ShoppingCartViewModel.Order.Id, session.Id, session.PaymentIntentId);
                    await _unitOfWork.SaveAsync();

                    Response.Headers.Add("Location", session.Url);

                    return new StatusCodeResult(303);
                }

                TempData["Success"] = "Order placed successfully.";
                return RedirectToAction(nameof(OrderConfirmation), new { orderId = ShoppingCartViewModel.Order.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while placing the order. Please try again.";
                return RedirectToAction(nameof(Summary));
            }
        }

        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            TbOrder? order = await _unitOfWork.Order.GetAsync(u => u.Id == orderId, includeProperties: "User");

            if (order != null)
            {
                if (order.PaymentStatus != SD.PaymentStatusDelayedPayment)
                {
                    //this is an order by customer
                    var service = new SessionService();
                    Session session = service.Get(order.SessionId);

                    if (session.PaymentStatus.ToLower() == "paid")
                    {
                        _unitOfWork.Order.UpdateStripePaymentId(orderId, session.Id, session.PaymentIntentId);
                        _unitOfWork.Order.UpdateStatus(orderId, SD.StatusApproved, SD.PaymentStatusApproved);
                        await _unitOfWork.SaveAsync();
                    }
                }

                IEnumerable<TbShoppingCart> shoppingCarts = await _unitOfWork.ShoppingCart.FindAllAsync(u => u.UserId == order.UserId);

                _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
                await _unitOfWork.SaveAsync();
            }

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
            if (cart.Quantity <= 50)
                return cart.Book.Price;

            if (cart.Quantity <= 100)
                return cart.Book.Price50;

            return cart.Book.Price100;
        }

        private void _CalcOrderTotal(ShoppingCartViewModel cartVM)
        {
            foreach (var cart in cartVM.LstShoppingCarts)
            {
                cart.Price = _GetPriceBasedOnQuantity(cart);

                cartVM.Order.OrderTotal += (cart.Price * cart.Quantity);
            }
        }

        private void _SaveCartQuantityInSession(int userId)
        {
            try
            {
                int cartQuantity = _unitOfWork.ShoppingCart.FindAllQueryable(x => x.UserId == userId).Count();
                HttpContext.Session.SetInt32(SD.SessionCart, cartQuantity);
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while retrieving the cart quantity.";
            }
        }
    }
}