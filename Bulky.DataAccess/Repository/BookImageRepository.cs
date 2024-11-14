using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;

namespace Bulky.DataAccess.Repository
{
	public class BookImageRepository : GenericRepository<TbBookImage>, IBookImageRepository
	{
		private readonly AppDbContext _context;

		public BookImageRepository(AppDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(TbBookImage entity)
		{
			_context.BookImages.Update(entity);
		}
	}
}
