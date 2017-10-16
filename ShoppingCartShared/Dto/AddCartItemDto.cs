using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart.Shared.Dto
{
    public class AddCartItemDto
    {
        public int Quantity { get; set; }
        public long ID { get; set; }
    }
}
