using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart.Shared.Model
{
    public class Product
    {
        public long ID { get; }
        public string Name { get; }
        public string Description { get; }
        public decimal Price { get; }
        public decimal Stock { get; set; }
    }
}
