using System;
using System.Runtime.Serialization;

namespace ShoppingCart.Repository.Exceptions
{
    public class CartCheckedOutException : Exception
    {
        public CartCheckedOutException()
        { }
    }
}