using System.Linq.Expressions;

namespace Bulky.DataAccess.Repository.IRepository
{
	public interface IGenericRepository<T> where T : class
	{
		// For chaining and advanced querying
		IQueryable<T> GetAllQueryable(string? includeProperties = null); // IQueryable<T>

		// For immediate data retrieval
		Task<IEnumerable<T>> GetAllAsync(string? includeProperties = null); // Task<IEnumerable<T>>

		// Same for filtered queries
		IQueryable<T> FindAllQueryable(Expression<Func<T, bool>> predicate, string? includeProperties = null); // IQueryable<T>

		Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null); // Task<IEnumerable<T>>

		Task<T?> GetByIdAsync(int id, string? includeProperties = null);
		Task<T?> GetAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null);
		Task AddAsync(T entity);
		void Remove(T entity);
		void RemoveRange(IEnumerable<T> entities);
	}

}
