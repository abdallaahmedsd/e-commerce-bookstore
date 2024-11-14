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
            Book = new BookRepository(_context);
            Company = new CompanyRepository(_context);
            ShoppingCart = new ShoppingCartRepository(_context);
            ApplicationUser = new ApplicationUserRepository(_context);
            Order = new OrderRepository(_context);
            OrderDetail = new OrderDetailRepository(_context);
            BookImage = new BookImageRepository(_context);
        }

        public ICategoryRepository Category { get; private set; }
        public IBookRepository Book { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
		public IOrderRepository Order { get; private set; }
		public IOrderDetailRepository OrderDetail { get; private set; }
        public IBookImageRepository BookImage { get; private set; }

		public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
