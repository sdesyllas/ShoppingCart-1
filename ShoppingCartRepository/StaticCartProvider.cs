using ShoppingCart.Shared;
using ShoppingCart.Shared.Model;
using System.Collections.Generic;

namespace ShoppingCartRepository
{
    public class StaticCartProvider : IDataProvider<Cart>
    {
        public List<Cart> Provide()
        {
            return new List<Cart>()
            {
                new Cart("cart1", new List<CartItem>()),
                new Cart("cart2", new List<CartItem>())
            };
        }
    }
}
