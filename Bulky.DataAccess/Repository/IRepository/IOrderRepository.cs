using Bulky.Models.Orders;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IOrderRepository : IGenericRepository<TbOrder>
    {
        void Update(TbOrder entity);

        void UpdateStatus(int id, string newStatus, string? newPaymentStatus = null);

        void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId);
    }
}
