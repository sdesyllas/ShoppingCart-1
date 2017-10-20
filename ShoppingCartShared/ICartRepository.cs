using ShoppingCart.Shared.Model;
using System;
using System.Threading.Tasks;

namespace ShoppingCart.Shared
{
    /// <summary>
    /// Repository for <see cref="Cart"/>
    /// </summary>
    public interface ICartRepository : IRepository<Cart>
    {
        /// <summary>
        /// Checks out 
        /// </summary>
        /// <param name="cartName"></param>
        /// <param name="productProvider"></param>
        /// <returns></returns>
        Task CheckoutAsync(string cartName, Func<long, Task<Product>> productProvider);

        Task AddItemToCartAsync(string cartName, Func<long, Task<Product>> productProvider, CartItem item);
    }
}
