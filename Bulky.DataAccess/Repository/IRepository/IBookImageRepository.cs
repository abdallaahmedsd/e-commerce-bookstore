using Bulky.Models;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IBookImageRepository : IGenericRepository<TbBookImage>
    {
        void Update(TbBookImage entity);
    }
}
