using System;

namespace ShoppingCart.Repository
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
