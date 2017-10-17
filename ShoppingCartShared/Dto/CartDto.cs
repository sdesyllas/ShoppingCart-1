using System.Collections.Generic;

namespace ShoppingCart.Shared.Dto
{
    public class CartDto
    {
        public string Name { get; set; }
        public bool IsCheckedOut { get; set; }
        public ICollection<CartItemDto> Items { get; set; }
    }
}
