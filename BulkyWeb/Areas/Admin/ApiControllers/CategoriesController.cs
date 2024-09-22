using Bulky.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.ApiControllers
{
	[Route("api/admin/[controller]")]
	[ApiController]
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
				return NotFound(new { success = false, message = $"({id}) is invlaid Id" });

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
				return StatusCode(500, new { success = false, message = "An error occurred while processing your request." });
			}
		}
	}
}
