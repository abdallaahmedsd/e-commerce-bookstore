using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.Orders;

namespace Bulky.DataAccess.Repository
{
    public class OrderRepository : GenericRepository<TbOrder>, IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(TbOrder entity)
        {
            _context.Orders.Update(entity);
        }
    }
}
