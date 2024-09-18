using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Repository
{
    public class CategoryRepository : GenericRepository<TbCategory>, ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TbCategory>> GetAllOrderedByDisplayOrder()
        {
            return await _context.Categories.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name).ToListAsync();
        }

        public void Update(TbCategory entity)
        {
            _context.Categories.Update(entity);
        }
    }
}
