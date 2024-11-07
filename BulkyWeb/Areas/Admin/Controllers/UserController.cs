using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.Identity;
using Bulky.Models.ViewModels.Admin;
using Bulky.Utility;
using BulkyWeb.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area(SD.Role_Admin)]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IReadOnlyRepository<UserListViewModel> _readOnlyRepository;
        private readonly AppDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserController(
            IReadOnlyRepository<UserListViewModel> readOnlyRepository,
            AppDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _readOnlyRepository = readOnlyRepository;
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> RoleManagement(int userId)
        {
            try
            {
                var userFromDb = _dbContext.Users.Find(userId);

                if (userFromDb == null)
                {
                    TempData["Error"] = $"There is no user with the Id = {userId}";
                    return NotFound();
                }

                UserPermissionsViewModel userPermissionsViewModel = new()
                {
                    Id = userFromDb.Id,
                    Name = userFromDb.Name,
                    CompanyId = userFromDb.CompanyId,
                };

                _GetAllRoles(userPermissionsViewModel);
                _GetAllCompanies(userPermissionsViewModel);

                await _GetUserRole(userPermissionsViewModel, userFromDb);

                return View(userPermissionsViewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while retrieving user info. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePermission(UserPermissionsViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please provide all required data.";
                return RedirectToAction(nameof(RoleManagement), new { userId = viewModel.Id });
            }

            try
            {
                var userFromDb = _dbContext.Users.Find(viewModel.Id);

                if (userFromDb == null)
                {
                    TempData["Error"] = $"There is no user with the Id = {viewModel.Id}";
                    return BadRequest();
                }

                // Get the user's current role (assuming single role)
                var currentRole = (await _userManager.GetRolesAsync(userFromDb)).FirstOrDefault();
                if (currentRole == null)
                {
                    TempData["Error"] = "User has no assigned role.";
                    return BadRequest("User has no assigned role.");
                }

                // Remove the user from their current role
                var removeResult = await _userManager.RemoveFromRoleAsync(userFromDb, currentRole);
                if (!removeResult.Succeeded)
                {
                    TempData["Error"] = "Error removing from current current role.";
                    return BadRequest("Error removing from current current role.");
                }

                // Ensure the new role exists in the system
                if (!await _roleManager.RoleExistsAsync(viewModel.Role))
                {
                    TempData["Error"] = "Role does not exist.";
                    return BadRequest("Role does not exist.");
                }

                // Add the user to the new role
                var addResult = await _userManager.AddToRoleAsync(userFromDb, viewModel.Role);
                if (!addResult.Succeeded)
                {
                    TempData["Error"] = "Error assigning new role.";
                    return BadRequest("Error assigning new role.");
                }

                // update user company
                if (viewModel.Role == SD.Role_Company)
                {
                    userFromDb.CompanyId = viewModel.CompanyId;
                }
                else
                {
                    userFromDb.CompanyId = null;
                }

                _dbContext.SaveChanges();

                TempData["success"] = "User role changed successfullt!";
                return View(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while changing user role. Please try again.";
                return RedirectToAction(nameof(Index));
            }
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
        public async Task<IActionResult> LockUnLock([FromBody] int userId)
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

        private async Task _GetUserRole(UserPermissionsViewModel userPermissionsViewModel, ApplicationUser user)
        {
            try
            {
                // Get the user's roles
                var roles = await _userManager.GetRolesAsync(user);
                userPermissionsViewModel.Role = roles.First();
            }
            catch
            {
                throw;
            }
        }

        private void _GetAllCompanies(UserPermissionsViewModel userPermissionsViewModel)
        {
            try
            {
                var companies = _dbContext.Companies
                    .Select(c => new CompanyForPermissionsViewModel
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                    .ToList();

                userPermissionsViewModel.Companies = companies;
            }
            catch
            {
                throw;
            }
        }

        private void _GetAllRoles(UserPermissionsViewModel userPermissionsViewModel)
        {
            try
            {
                var roles = _roleManager.Roles
                    .Select(x => x.Name)
                    .Select(x =>
                    new SelectListItem
                    {
                        Text = x,
                        Value = x
                    }).ToList();

                userPermissionsViewModel.Roles = roles;
            }
            catch
            {
                throw;
            }
        }
    }
}
