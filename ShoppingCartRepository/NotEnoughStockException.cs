using System;

namespace ShoppingCart.Repository
{
    public class NotEnoughStockException : Exception
    {
        public NotEnoughStockException()
        { }
    }
}