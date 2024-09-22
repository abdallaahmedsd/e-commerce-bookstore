using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.ViewModels.Admin.Books;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.ApiControllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
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

        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var lstBooks = await _readOnlyRepository.GetAllAsync();
                return Ok(new { data = lstBooks });
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                // _logger.LogError(ex, "An error occurred while retrieving categories");

                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return NotFound(new { success = false, message = $"({id}) is invlaid Id" });

            try
            {
                var book = await _unitOfWork.Book.GetByIdAsync(id);

                if (book == null)
                    return NotFound(new {success = false, message = $"There's no book with Id = ({id})"});

                // delete old image if the user selected a new image
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
