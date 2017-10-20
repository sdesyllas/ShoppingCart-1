using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart.Shared.Dto
{
    /// <summary>
    /// Dto for adding item to <see cref="Cart"/>
    /// </summary>
    public class AddCartItemDto
    {
        public int Quantity { get; set; }
        public long ProductId { get; set; }
    }
}
