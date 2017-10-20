using System.Collections.Generic;
using ShoppingCart.Shared.Model;

namespace ShoppingCart.Shared.Dto
{
    /// <summary>
    /// Dto for <see cref="Cart"/>
    /// </summary>
    public class CartDto
    {
        public string Name { get; set; }
        public bool IsCheckedOut { get; set; }
        public ICollection<CartItemDto> Items { get; set; }
    }
}
