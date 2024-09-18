using Bulky.Models;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IGenericRepository<TbProduct>
    {
        void Update(TbProduct entity);
    }
}
