using Bulky.DataAccess.Repository.IRepository;
using BulkyWeb.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _unitOfWork.Category.GetAllAsync();

                var lstCategories = new List<CategoryListDto>();
                foreach (var category in categories)
                {
                    lstCategories.Add(new CategoryListDto 
                    {   
                        Id = category.Id,
                        Name = category.Name
                    });
                }

                return Ok(lstCategories);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // _logger.LogError(ex, "An error occurred while retrieving categories");

                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

    }
}
