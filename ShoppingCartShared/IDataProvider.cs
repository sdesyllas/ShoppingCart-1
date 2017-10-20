using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingCart.Shared
{
    /// <summary>
    /// Data source for <typeparamref name="T"/> collection
    /// </summary>
    /// <typeparam name="T">Type of repository items</typeparam>
    public interface IDataProvider<T>
    {
        /// <summary>
        /// Provides source collection of for <typeparamref name="T"/>
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> ProvideAsync();
    }
}
