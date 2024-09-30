using Bulky.Models;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository : IGenericRepository<TbShoppingCart>
    {
        void Update(TbShoppingCart entity);
    }
}
