namespace ShoppingCart.Shared.Dto
{
    /// <summary>
    /// Dto for cart item product
    /// </summary>
    public class CartProductDto
    {
        public long Id { get; set; }
        public string Name { get; set;  }
        public string Description { get; set;  }
        public decimal Price { get; set;  }
    }
}
