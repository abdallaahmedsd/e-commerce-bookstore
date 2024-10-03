using Bulky.Models.Orders;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IOrderDetailRepository : IGenericRepository<TbOrderDetail>
    {
        void Update(TbOrderDetail entity);
    }
}
