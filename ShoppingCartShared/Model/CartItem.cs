namespace ShoppingCart.Shared.Model
{
    /// <summary>
    /// Model for shopping basket element
    /// </summary>
    public class CartItem
    {
        public int Quantity { get; set; }
        public long ProductId { get; set; }
    }
}
