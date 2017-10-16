using System.Threading.Tasks;

namespace ShoppingCart.Shared
{
    public interface IRepository<T>
    {
        Task<T> GetByName(string name);
    }
}
