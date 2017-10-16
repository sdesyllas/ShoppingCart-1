using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart.Shared.Dto
{
    public class CartDto
    {
        public string Name { get; set; }

        public List<CartItemDto> Items { get; set; }
    }
}
