using System;
using System.Runtime.Serialization;

namespace ShoppingCart.Repository
{
    public class CartCheckedOutException : Exception
    {
        public CartCheckedOutException()
        { }
    }
}