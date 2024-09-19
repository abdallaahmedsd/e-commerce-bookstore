using Bulky.Models;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IBookRepository : IGenericRepository<TbBook>
    {
        void Update(TbBook entity);
    }
}
