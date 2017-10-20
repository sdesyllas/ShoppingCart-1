using System;
using System.Threading.Tasks;

namespace ShoppingCart.Shared
{
    /// <summary>
    /// Repository for <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">Type of repository items</typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Obtains a <typeparamref name="T"/> by a predicate
        /// </summary>
        /// <param name="predicate">Predicate for filtering</param>
        /// <returns>Found <typeparamref name="T"/> object</returns>
        Task<T> GetAsync(Func<T, bool> predicate);
    }
}
