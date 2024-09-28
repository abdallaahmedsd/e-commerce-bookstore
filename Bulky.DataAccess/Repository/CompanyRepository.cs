using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;

namespace Bulky.DataAccess.Repository
{
	public class CompanyRepository : GenericRepository<TbCompany>, ICompanyRepository
	{
		private readonly ApplicationDbContext _context;

		public CompanyRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(TbCompany entity)
		{
			_context.Companies.Update(entity);
		}
	}
}
