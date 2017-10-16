using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCartRepository
{
    public interface IDataProvider<T>
    {
        List<T> Provide();
    }
}
