using Bulky.Models;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        void Update(Category entity);
    }
}
