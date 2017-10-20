using ShoppingCart.Shared;
using ShoppingCart.Shared.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingCart.Repository
{
    /// <summary>
    /// Provides predefinded <see cref="Cart"/> collection.
    /// </summary>
    public class StaticCartProvider : IDataProvider<Cart>
    {
        /// <summary>
        /// Provides predefined <see cref="Cart"/> collection as async operation
        /// </summary>
        /// <returns><see cref="Cart"/> collection as async operation</returns>
        public async Task<IEnumerable<Cart>> ProvideAsync()
        {
            return await Task.FromResult(new List<Cart>()
            {
                new Cart("cart1", new List<CartItem>()),
                new Cart("cart2", new List<CartItem>())
            } as IEnumerable<Cart>);
        }
    }
}
