using System;

namespace ShoppingCart.Repository.Exceptions
{
    public class ProdcutNotFoundException : Exception
    {
        internal ProdcutNotFoundException(Exception innerException)
            : base("Cart not found", innerException)
        { }

        public ProdcutNotFoundException()
        { }
    }
}
