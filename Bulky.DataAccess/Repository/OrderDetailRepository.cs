using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Orders;

namespace Bulky.DataAccess.Repository
{
    public class OrderDetailRepository : GenericRepository<TbOrderDetail>, IOrderDetailRepository
    {
        private readonly AppDbContext _context;

        public OrderDetailRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(TbOrderDetail entity)
        {
            _context.OrderDetails.Update(entity);
        }
    }
}
