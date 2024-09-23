using Bulky.Models;
using Bulky.Models.Identity;
using Bulky.Models.ViewModels.Admin.Books;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<TbCategory> Categories { get; set; }

        public DbSet<TbBook> Books { get; set; }   
        
        public DbSet<BookListViewModel> BookListView {  get; set; } 

        public DbSet<ApplicationRole> ApplicationRoles { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
