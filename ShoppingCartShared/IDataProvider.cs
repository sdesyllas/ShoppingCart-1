using System.Collections.Generic;

namespace ShoppingCart.Shared
{
    public interface IDataProvider<T>
    {
        IEnumerable<T> Provide();
    }
}
