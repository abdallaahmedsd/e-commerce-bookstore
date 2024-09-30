using Bulky.Models;
using Bulky.Models.Identity;
using Bulky.Models.ViewModels.Admin.Books;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<TbCategory> Categories { get; set; }
        public DbSet<TbBook> Books { get; set; }   
        public DbSet<TbCompany> Companies { get; set; } 
        public DbSet<BookListViewModel> BookListView {  get; set; } 
        public DbSet<TbShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
