namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IBookRepository Book { get; }
        ICompanyRepository Company { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IOrderRepository Order { get; }
        IOrderDetailRepository OrderDetail { get; }
        IBookImageRepository BookImage { get; }
        Task SaveAsync();
    }
}
