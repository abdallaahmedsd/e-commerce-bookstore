using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Models.ViewModels.Customer;
using Bulky.Utility;
using BulkyWeb.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReadOnlyRepository<BookHomeViewModel> _readOnlyRepository;

        public HomeController(IUnitOfWork unitOfWork, IReadOnlyRepository<BookHomeViewModel> readOnlyRepository, ILogger<HomeController> logger)
        {
            _unitOfWork = unitOfWork;
            _readOnlyRepository = readOnlyRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ClaimsIdentity claimsIdentity = (ClaimsIdentity)User?.Identity;
                var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

                if (claim != null)
                {
                    int userId = int.Parse(claim.Value);
                    // update number of shopping cart items
                    _SaveCartQuantityInSession(userId);
                }


                var lstBooks = await _readOnlyRepository.GetAllAsync();

                // order them randomly 
                lstBooks = lstBooks.OrderBy(x => Guid.NewGuid()).ToList();
                return View(lstBooks);
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "An error occurred while retrieving the books.");
                TempData["error"] = "An error occurred while retrieving the books.";
                return View("Error");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Details(int bookId)
        {
            if (bookId <= 0)
                return NotFound();

            try
            {
                var bookModel = await _unitOfWork.Book.GetByIdAsync(bookId, "Category");

                if (bookModel == null)
                    return NotFound();

                BookDetailsViewModel shoppingCartViewModel = new();

                Mapper.Map(bookModel, shoppingCartViewModel);

                // set the quantity to one by default
                shoppingCartViewModel.Quantity = 1;

                return View(shoppingCartViewModel);
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "An error occurred while retrieving book details.");
                TempData["error"] = "An error occurred while retrieving book details.";
                return View("Error");
            }
        }

        [HttpPost]
        [Authorize]
        [ActionName("Details")]
        public async Task<IActionResult> AddToCart(AddToCartViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ClaimsIdentity claimsIdentity = (ClaimsIdentity)User?.Identity;
                    int userId = int.Parse(claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value);

                    var cartFromDb = await _unitOfWork.ShoppingCart.GetAsync(x => x.UserId == userId && x.BookId == viewModel.BookId);

                    if (cartFromDb != null)
                    {
                        // update the quantity
                        cartFromDb.Quantity += viewModel.Quantity;
                        _unitOfWork.ShoppingCart.Update(cartFromDb);
                    }
                    else
                    {
                        // create new 
                        var shoppingCart = new TbShoppingCart
                        {
                            UserId = userId,
                            Quantity = viewModel.Quantity,
                            BookId = viewModel.BookId
                        };

                        await _unitOfWork.ShoppingCart.AddAsync(shoppingCart);
                    }

                    await _unitOfWork.SaveAsync();

                    // update number of shopping cart items
                    _SaveCartQuantityInSession(userId);

                    TempData["success"] = "Item added to the cart successfully!";
                    return RedirectToAction(nameof(Index));
                }

                return View("Details", new { viewModel.BookId });
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "An error occurred while adding the book to the cart.");
                TempData["error"] = "An error occurred while adding the book to the cart.";
                return View("Error");
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
