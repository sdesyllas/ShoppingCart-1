using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart.Shared.Dto
{
    public class Product
    {
        public long ID { get; set; }
        public string Name { get; set;  }
        public string Description { get; set;  }
        public decimal Price { get; set;  }
        public int Stock { get; set; }

    }
}
