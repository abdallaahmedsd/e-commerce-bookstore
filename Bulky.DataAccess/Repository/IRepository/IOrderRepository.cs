using Bulky.Models.Orders;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IOrderRepository : IGenericRepository<TbOrder>
    {
        void Update(TbOrder entity);
    }
}
