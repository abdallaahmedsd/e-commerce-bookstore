using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;

namespace Bulky.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Category = new CategoryRepository(_context);
        }

        public ICategoryRepository Category { get; private set; }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
