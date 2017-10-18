using System;

namespace ShoppingCart.Repository
{
    public class CartNotFoundException : Exception
    {
        internal CartNotFoundException(Exception innerException)
            : base("Cart not found", innerException)
        { }

        public CartNotFoundException()
        { }
    }
}
