using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels.Admin;
using Bulky.Utility;
using BulkyWeb.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area(SD.Role_Admin)]
	[Authorize(Roles = SD.Role_Admin)]
	public class UserController : Controller
	{
		private readonly IReadOnlyRepository<UserListViewModel> _readOnlyRepository;
		private readonly AppDbContext _dbContext;

		public UserController(IReadOnlyRepository<UserListViewModel> readOnlyRepository, AppDbContext dbContext)
		{
			_readOnlyRepository = readOnlyRepository;
			_dbContext = dbContext;
		}

		public IActionResult Index()
		{
			return View();
		}

		#region API Calls
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var lstUsers = await _readOnlyRepository.GetAllAsync();

				return Ok(new { success = true, data = lstUsers });
			}
			catch (Exception ex)
			{
				// Log the exception details (optional)
				return StatusCode(500, new { success = false, message = "An error occurred while retrieving users." });
			}
		}
		#endregion
	}
}
