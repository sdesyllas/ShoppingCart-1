using System;
using System.Threading.Tasks;

namespace ShoppingCart.Shared
{
    public interface IRepository<T>
    {
        Task<T> GetAsync(Func<T, bool> predicate);
    }
}
