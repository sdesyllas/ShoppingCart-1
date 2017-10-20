using System;
using System.Collections.Generic;
using System.Text;
using ShoppingCart.Shared.Model;

namespace ShoppingCart.Shared.Dto
{
    /// <summary>
    /// Dto for <see cref="CartItem"/>
    /// </summary>
    public class CartItemDto
    {
        public decimal Quantity { get; set; }
        public CartProductDto Product { get; set; }
    }
}
