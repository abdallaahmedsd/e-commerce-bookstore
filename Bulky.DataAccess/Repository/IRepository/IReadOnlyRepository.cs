namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IReadOnlyRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
    }
}
