using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart.Shared.Dto
{
    public class CartItem
    {
        public decimal Quantity { get; set; }
        public Product Product { get; set; }
    }
}
