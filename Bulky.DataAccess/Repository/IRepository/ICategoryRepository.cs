using Bulky.Models;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository : IGenericRepository<TbCategory>
    {
        Task<IEnumerable<TbCategory>> GetAllOrderedByDisplayOrderAsync();
        void Update(TbCategory entity);
    }
}
