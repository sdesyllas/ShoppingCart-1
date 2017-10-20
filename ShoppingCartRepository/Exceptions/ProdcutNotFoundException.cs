using System;
using ShoppingCart.Shared.Model;

namespace ShoppingCart.Repository.Exceptions
{
    /// <summary>
    /// Defines exception for not found <see cref="Product"/>
    /// </summary>
    public class ProdcutNotFoundException : Exception
    {
        internal ProdcutNotFoundException(Exception innerException)
            : base("Cart not found", innerException)
        { }

        public ProdcutNotFoundException()
        { }
    }
}
