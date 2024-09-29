using Bulky.DataAccess.Repository.IRepository;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.ApiControllers
{
	[Route("api/admin/[controller]")]
	[ApiController]
	// [Authorize(Roles = SD.Role_Admin)]
	public class CategoriesController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;

		public CategoriesController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
	
		[HttpDelete("{id:int}")]
		public async Task<IActionResult> Delete(int id)
		{
			if (id <= 0)
				return BadRequest(new { success = false, message = $"({id}) is an invalid Id" });

			try
			{
				var category = await _unitOfWork.Category.GetByIdAsync(id);

				if (category == null)
					return NotFound(new { success = false, message = $"There's no category with Id = ({id})" });

				_unitOfWork.Category.Remove(category);
				await _unitOfWork.SaveAsync();
				return Ok(new { success = true, message = "Category deleted successfully!" });
			}
			catch (Exception ex)
			{
				// Log exception details (optional) for debugging
				return StatusCode(500, new { success = false, message = "An error occurred while processing your request.", error = ex.Message });
			}
		}
	}

}
