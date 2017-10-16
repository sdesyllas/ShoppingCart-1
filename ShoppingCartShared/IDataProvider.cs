using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingCart.Shared
{
    public interface IDataProvider<T>
    {
        Task<IEnumerable<T>> Provide();
    }
}
