using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TbCategory> Categories { get; set; }

        public DbSet<TbBook> Books { get; set; }   
        
        public DbSet<BookListViewModel> BookListView {  get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
