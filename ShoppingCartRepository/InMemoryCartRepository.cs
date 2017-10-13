using ShoppingCart.Shared;
using ShoppingCart.Shared.Model;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCartRepository
{
    public class InMemoryCartRepository : IRepository<Cart>
    {
        private readonly List<Cart> baskets = new List<Cart>()
        {
            new Cart("cart1", new List<CartItem>()),
            new Cart("cart2", new List<CartItem>())
        };

        public Cart GetByName(string name)
        {
            return baskets.FirstOrDefault(x => x.Name == name);
        }
    }
}
