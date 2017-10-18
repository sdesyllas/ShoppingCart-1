using ShoppingCart.Shared.Model;
using System;
using System.Threading.Tasks;

namespace ShoppingCart.Shared
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task CheckoutAsync(string cartName, Func<long, Task<Product>> productProvider);

        Task AddItemToCartAsync(string cartName, Func<long, Task<Product>> productProvider, CartItem item);
    }
}
