using System.Collections.Generic;

namespace ShoppingCart.Shared.Model
{
    /// <summary>
    /// Model for a shopping basket
    /// </summary>
    public class Cart
    {
        public string Name { get; private set; }

        public ICollection<CartItem> Items { get; set; }

        public bool IsCheckedOut { get; set; }

        public Cart(string name, ICollection<CartItem> items)
        {
            Name = name;
            Items = items;
        }
    }
}
