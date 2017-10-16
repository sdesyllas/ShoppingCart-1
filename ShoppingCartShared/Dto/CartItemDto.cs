using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart.Shared.Dto
{
    public class CartItemDto
    {
        public decimal Quantity { get; set; }
        public CartProductDto Product { get; set; }
    }
}
