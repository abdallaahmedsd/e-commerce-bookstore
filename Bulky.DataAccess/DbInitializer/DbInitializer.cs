using Bulky.DataAccess.Data;
using Bulky.Models;
using Bulky.Models.Common;
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
						},
						LockoutEnabled = false,
					};

					// Attempt to create the new admin user with a preset password
					var result = _userManager.CreateAsync(user, "Admin123@").GetAwaiter().GetResult();

					// If user creation succeeds, assign them the Admin role
					if (result.Succeeded)
					{
						_userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
					}
				}

				// seed database 
				// I checked the categories because the books depend on them
				if (!_dbContext.Categories.Any())
				{
					_dbContext.Categories.AddRange(_SeedCategory());
					_dbContext.SaveChanges();

					_dbContext.Books.AddRange(_SeedBooks());
					_dbContext.SaveChanges();

					_dbContext.BookImages.AddRange(_SeedBookImages());
					_dbContext.SaveChanges();
				}

				// seed database with companies
				if (!_dbContext.Companies.Any())
				{
					_dbContext.Companies.AddRange(_SeedCompanies());
					_dbContext.SaveChanges();
				}

			}
			catch (Exception ex)
			{
			}
		}

		private static List<TbCategory> _SeedCategory()
		{
			return
			[
				new TbCategory { Name = "Action", DisplayOrder = 1},
				new TbCategory { Name = "SciFi", DisplayOrder = 2},
				new TbCategory { Name = "History", DisplayOrder = 3}
			];
		}

		private static List<TbBook> _SeedBooks() => new List<TbBook>
		{
			new() {
				Title = "Fortune of Time",
				Author = "Billy Spark",
				Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.",
				ISBN = "SWD-9999-001A",
				ListPrice = 99,
				Price = 90,
				Price50 = 85,
				Price100 = 80,
				CategoryId = 1,
			},
			new() {
				Title = "Dark Skies",
				Author = "Nancy Hoover",
				Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.",
				ISBN = "CAW-7777-01B",
				ListPrice = 40,
				Price = 30,
				Price50 = 25,
				Price100 = 20,
				CategoryId = 1,
			},
			new() {
				Title = "Vanish in the Sunset",
				Author = "Julian Button",
				Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.",
				ISBN = "RIT-0555-01C",
				ListPrice = 55,
				Price = 50,
				Price50 = 40,
				Price100 = 35,
				CategoryId = 2,
			},
			new() {
				Title = "Cotton Candy",
				Author = "Abby Muscles",
				Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.",
				ISBN = "WS3-3333-3301D",
				ListPrice = 70,
				Price = 65,
				Price50 = 60,
				Price100 = 55,
				CategoryId = 3,
			},
			new() {
				Title = "Rock in the Ocean",
				Author = "Ron Parker",
				Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.",
				ISBN = "SOT-1111-1101E",
				ListPrice = 30,
				Price = 27,
				Price50 = 25,
				Price100 = 20,
				CategoryId = 2,
			},
			new() {
				Title = "Leaves and Wonders",
				Author = "Laura Phantom",
				Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt.",
				ISBN = "FOT-0000-0001F",
				ListPrice = 25,
				Price = 23,
				Price50 = 22,
				Price100 = 20,
				CategoryId = 2,
			}
		};

		private List<TbBookImage> _SeedBookImages()
		{
			return
			[
				new TbBookImage { ImageUrl = @"\uploads\images\books\book-1\fortune of time.jpg", BookId = 1, IsMainImage = true},
				new TbBookImage { ImageUrl = @"\uploads\images\books\book-1\fortune of time back.jpg", BookId = 1, IsMainImage = false},

				new TbBookImage { ImageUrl = @"\uploads\images\books\book-2\dark skies.jpg", BookId = 2, IsMainImage = true},
				new TbBookImage { ImageUrl = @"\uploads\images\books\book-2\dark skies back.jpg", BookId = 2, IsMainImage = false},

				new TbBookImage { ImageUrl = @"\uploads\images\books\book-3\vanish in the sunset.jpg", BookId = 3, IsMainImage = true},
				new TbBookImage { ImageUrl = @"\uploads\images\books\book-3\vanish in the sunset back.jpg", BookId = 3, IsMainImage = false},

				new TbBookImage { ImageUrl = @"\uploads\images\books\book-4\cotton candy.jpg", BookId = 4, IsMainImage = true},
				new TbBookImage { ImageUrl = @"\uploads\images\books\book-4\cotton candy back.jpg", BookId = 4, IsMainImage = false},

				new TbBookImage { ImageUrl = @"\uploads\images\books\book-5\rock in the ocean.jpg", BookId = 5, IsMainImage = true},
				new TbBookImage { ImageUrl = @"\uploads\images\books\book-5\rock in the ocean back.jpg", BookId = 5, IsMainImage = false},

				new TbBookImage { ImageUrl = @"\uploads\images\books\book-6\leaves and wonders.jpg", BookId = 6, IsMainImage = true},
				new TbBookImage { ImageUrl = @"\uploads\images\books\book-6\leaves and wonders back.jpg", BookId = 6, IsMainImage = false},
			];
		}

		private List<TbCompany> _SeedCompanies()
		{
			return
			[
				new TbCompany
				{
					Name = "Tech Innovations",
					PhoneNumber = "30282123",
					AddressInfo = new AddressInfo
					{
						City = "Doha",
						State = "Qatar",
						StreetAddress = "123 Tech Street",
						PostalCode = "10001"
					}
				},
				new TbCompany
				{
					Name = "Global Solutions",
					PhoneNumber = "40011255",
					AddressInfo = new AddressInfo
					{
						City = "New York",
						State = "NY",
						StreetAddress = "456 Global Ave",
						PostalCode = "10002"
					}
				},
				new TbCompany
				{
					Name = "Future Enterprises",
					PhoneNumber = "50077777",
					AddressInfo = new AddressInfo
					{
						City = "Berlin",
						State = "Berlin",
						StreetAddress = "789 Future Blvd",
						PostalCode = "10003"
					}
				}
			];
		}
	}
}
