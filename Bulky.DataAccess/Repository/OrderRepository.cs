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

		public void UpdateStatus(int id, string newStatus, string? newPaymentStatus = null)
		{
			TbOrder? order = _context.Orders.Find(id);

            if (order != null) 
            {
                order.OrderStatus = newStatus;

                if(!string.IsNullOrWhiteSpace(newPaymentStatus))
                    order.PaymentStatus = newPaymentStatus;
            }
		}

		public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
		{
			TbOrder? order = _context.Orders.Find(id);

			if (order != null)
			{
				if (!string.IsNullOrWhiteSpace(sessionId))
					order.SessionId = sessionId;

				if (!string.IsNullOrWhiteSpace(sessionId))
					order.PaymentIntentId = paymentIntentId;
			}
		}
	}
}
