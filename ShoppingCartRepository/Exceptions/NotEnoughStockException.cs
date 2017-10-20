using System;
using ShoppingCart.Shared.Model;

namespace ShoppingCart.Repository.Exceptions
{
    /// <summary>
    /// Defines exception for insufficient <see cref="Product"/> stock.
    /// </summary>
    public class NotEnoughStockException : Exception
    {
        public NotEnoughStockException()
        { }
    }
}