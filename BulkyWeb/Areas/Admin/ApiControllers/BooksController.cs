using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.ViewModels.Admin.Books;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.ApiControllers
{
	[Route("api/admin/[controller]")]
	[ApiController]
	[Authorize(Roles = SD.Role_Admin)]
	public class BooksController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IReadOnlyRepository<BookListViewModel> _readOnlyRepository;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public BooksController(IUnitOfWork unitOfWork, IReadOnlyRepository<BookListViewModel> readOnlyRepository, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_readOnlyRepository = readOnlyRepository;
			_webHostEnvironment = webHostEnvironment;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var lstBooks = await _readOnlyRepository.GetAllAsync();
				return Ok(new { success = true, data = lstBooks });
			}
			catch (Exception ex)
			{
				// Log the exception details (optional)
				return StatusCode(500, new { success = false, message = "An error occurred while retrieving books." });
			}
		}

		[HttpDelete("{id:int}")]
		public async Task<IActionResult> Delete(int id)
		{
			if (id <= 0)
				return BadRequest(new { success = false, message = $"({id}) is an invalid Id" });

			try
			{
				var book = await _unitOfWork.Book.GetByIdAsync(id);

				if (book == null)
					return NotFound(new { success = false, message = $"There's no book with Id = ({id})" });

				// Delete old image
				string wwwRootPath = _webHostEnvironment.WebRootPath;
				string oldImagePath = Path.Combine(wwwRootPath, book.ImageUrl);
				if (System.IO.File.Exists(oldImagePath))
				{
					System.IO.File.Delete(oldImagePath);
				}

				_unitOfWork.Book.Remove(book);
				await _unitOfWork.SaveAsync();
				return Ok(new { success = true, message = "Book deleted successfully!" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { success = false, message = "An error occurred while processing your request." });
			}
		}
	}
}
