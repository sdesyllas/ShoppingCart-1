namespace ShoppingCart.Shared
{
    public interface IQueryableByIdRepository<T> : IRepository<T>
    {
        T GetById(long id);
    }
}
