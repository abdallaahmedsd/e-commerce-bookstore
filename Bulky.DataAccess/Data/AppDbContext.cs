using Bulky.Models;
using Bulky.Models.Identity;
using Bulky.Models.Orders;
using Bulky.Models.ViewModels.Admin.Books;
using Bulky.Models.ViewModels.Customer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TbCategory> Categories { get; set; }
        public DbSet<TbBook> Books { get; set; }   
        public DbSet<TbCompany> Companies { get; set; } 
        public DbSet<BookListViewModel> BookListView {  get; set; } 
        public DbSet<BookHomeViewModel> BookHomeView {  get; set; } 
        public DbSet<TbShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<TbOrder> Orders { get; set; }
        public DbSet<TbOrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
