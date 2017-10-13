namespace ShoppingCart.Shared
{
    public interface IRepository<T>
    {
        T GetByName(string name);
    }
}
