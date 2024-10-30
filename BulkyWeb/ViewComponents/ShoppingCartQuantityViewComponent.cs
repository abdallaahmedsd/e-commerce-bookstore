using Bulky.DataAccess.Repository.IRepository;
using Bulky.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.ViewComponents
{
    public class ShoppingCartQuantityViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartQuantityViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                ClaimsIdentity claimsIdentity = (ClaimsIdentity)User?.Identity;
                var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

                if (claim != null)
                {
                    int userId = int.Parse(claim.Value);

                    int? cartQuantityFromSession = HttpContext.Session.GetInt32(SD.SessionCart);

                    if (!cartQuantityFromSession.HasValue)
                    {
                        int cartQuantityFromDb = _unitOfWork.ShoppingCart.FindAllQueryable(x => x.UserId == userId).Count();
                        HttpContext.Session.SetInt32(SD.SessionCart, cartQuantityFromDb);
                    }

                    cartQuantityFromSession = HttpContext.Session.GetInt32(SD.SessionCart) ?? 0;

                    return View(cartQuantityFromSession.Value);
                }
                else
                {
                    HttpContext.Session.Clear();
                    return View(0);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while retrieving the cart quantity.";
                return View(0);
            }
        }
    }
}
