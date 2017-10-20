using System;
using ShoppingCart.Shared.Model;

namespace ShoppingCart.Repository.Exceptions
{
    /// <summary>
    /// Defines exception for operation on checked out <see cref="Cart"/>.
    /// </summary>
    public class CartCheckedOutException : Exception
    {
        public CartCheckedOutException()
        { }
    }
}