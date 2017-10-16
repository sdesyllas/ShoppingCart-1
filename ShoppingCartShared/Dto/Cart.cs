using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart.Shared.Dto
{
    public class Cart
    {
        public string Name { get; set; }

        public List<CartItem> Items { get; set; }
    }
}
