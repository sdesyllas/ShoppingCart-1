using System;
using ShoppingCart.Shared.Model;

namespace ShoppingCart.Repository.Exceptions
{
    /// <summary>
    /// Defines exception for not found <see cref="Cart"/>.
    /// </summary>
    public class CartNotFoundException : Exception
    {
        internal CartNotFoundException(Exception innerException)
            : base("Cart not found", innerException)
        { }

        public CartNotFoundException()
        { }
    }
}
