using System.Threading.Tasks;

namespace ShoppingCart.Shared
{
    public interface IQueryableByIdRepository<T> : IRepository<T>
    {
        Task<T> GetByIdAsync(long id);
    }
}
