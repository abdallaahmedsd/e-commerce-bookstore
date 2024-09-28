using Bulky.Models;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface ICompanyRepository : IGenericRepository<TbCompany>
    {
        void Update(TbCompany entity);
    }
}
