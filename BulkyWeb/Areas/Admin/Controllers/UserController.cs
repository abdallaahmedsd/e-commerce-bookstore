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

		[HttpPost]
		public async Task<IActionResult> LockUnLock([FromBody]int userId)
		{
			try
			{
				var userFromDb = _dbContext.Users.Find(userId);

				if (userFromDb == null)
				{
					return Json(new { success = false, message = $"There is no user with the Id = {userId}" });
				}

				// if this user is the main user (admin) return false because we can't lock the main user
				if (!userFromDb.LockoutEnabled)
				{
					return Json(new { success = false, message = "Ooobs! You cannot lock this user, they are the main user in this system." });
				}	

				string result = "";

				// if the user is already locked we need to unlock them, otherwise lock the user
				if (userFromDb.LockoutEnd > DateTime.Now) 
				{
					// unlock the user
					userFromDb.LockoutEnd = DateTime.Now;
					result = "unlocked";
				}
				else
				{
					// lock the user for a 1000 years
					userFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
					result = "locked";
				}

				await _dbContext.SaveChangesAsync();

				return Json(new { success = true, message = $"User has been {result} successfully!" });
			}
			catch (Exception ex)
			{
				// Log the exception details (optional)
				return StatusCode(500, new { success = false, message = "An error occurred while lock/unLock user." });
			}
		}
		#endregion
	}
}
