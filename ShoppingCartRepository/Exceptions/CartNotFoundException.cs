using System;

namespace ShoppingCart.Repository.Exceptions
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
