using Bulky.DataAccess.Data;
using Bulky.Models.Identity;
using Bulky.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.DbInitializer
{
	public class DbInitializer : IDbInitializer
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<ApplicationRole> _roleManager;
		private readonly AppDbContext _dbContext;

		public DbInitializer(AppDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
		{
			_dbContext = dbContext;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public void Initialize()
		{
			try
			{
				// Apply any pending migrations to ensure the database is up-to-date
				if (_dbContext.Database.GetPendingMigrations().Any())
				{
					_dbContext.Database.Migrate();
				}

				// Check if any roles exist; if not, create necessary roles
				if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
				{
					_roleManager.CreateAsync(new ApplicationRole(SD.Role_Admin)).GetAwaiter().GetResult();
					_roleManager.CreateAsync(new ApplicationRole(SD.Role_Customer)).GetAwaiter().GetResult();
					_roleManager.CreateAsync(new ApplicationRole(SD.Role_Employee)).GetAwaiter().GetResult();
					_roleManager.CreateAsync(new ApplicationRole(SD.Role_Company)).GetAwaiter().GetResult();
				}

				// Check if any users exist; if not, create a default admin user
				if (!_dbContext.ApplicationUsers.Any())
				{
					ApplicationUser user = new()
					{
						Name = "Abdalla Ahmed",
						UserName = "admin@gmail.com",
						Email = "admin@gmail.com",
						PhoneNumber = "123456789",
						AddressInfo =
						{
							StreetAddress = "123 Main St",
							City = "Khartoum",
							State = "Sudan",
							PostalCode = "11111"
						}
					};

					// Attempt to create the new admin user with a preset password
					var result = _userManager.CreateAsync(user, "Admin123@").GetAwaiter().GetResult();

					// If user creation succeeds, assign them the Admin role
					if (result.Succeeded)
					{
						_userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
					}
				}
			}
			// Handle invalid operations such as accessing an uninitialized object
			catch (InvalidOperationException invOpEx)
			{
			}
			// Catch errors during database updates, such as constraint violations
			catch (DbUpdateException dbEx)
			{
			}
			// Catch null argument errors, typically if a required service is not initialized
			catch (ArgumentNullException argNullEx)
			{
			}
			// Handle canceled asynchronous tasks, potentially from timeout issues
			catch (TaskCanceledException taskEx)
			{
			}
			// Catch any other unexpected errors
			catch (Exception ex)
			{
			}
		}
	}
}
