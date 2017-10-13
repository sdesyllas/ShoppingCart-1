using System.Collections.Generic;

namespace ShoppingCart.Shared.Model
{
    public class Cart
    {
        public string Name { get; private set; }

        public List<CartItem> Items { get; set; }

        public Cart(string name, List<CartItem> items)
        {
            Name = name;
            Items = items;
        }
    }
}
