using System;
using System.Runtime.Serialization;

namespace ShoppingCart.Repository
{
    public class NotEnoughStockException : Exception
    {
        public NotEnoughStockException()
        { }
    }
}