namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }

        IBookRepository Book { get; }

        ICompanyRepository Company { get; }

        Task SaveAsync();
    }
}
