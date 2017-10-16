using ShoppingCart.Shared;
using ShoppingCart.Shared.Model;
using System.Collections.Generic;

namespace ShoppingCart.Repository
{
    public class StaticCartProvider : IDataProvider<Cart>
    {
        public IEnumerable<Cart> Provide()
        {
            return new List<Cart>()
            {
                new Cart("cart1", new List<CartItem>()),
                new Cart("cart2", new List<CartItem>())
            };
        }
    }
}
