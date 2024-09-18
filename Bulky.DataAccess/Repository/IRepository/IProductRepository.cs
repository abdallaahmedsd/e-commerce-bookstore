using Bulky.Models;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        void Update(Product entity);
    }
}
