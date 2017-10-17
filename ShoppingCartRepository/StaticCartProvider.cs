using ShoppingCart.Shared;
using ShoppingCart.Shared.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoppingCart.Repository
{
    public class StaticCartProvider : IDataProvider<Cart>
    {
        public async Task<IEnumerable<Cart>> ProvideAsync()
        {
            return await Task.FromResult(new List<Cart>()
            {
                new Cart("cart1", new List<CartItem>()),
                new Cart("cart2", new List<CartItem>())
            } as IEnumerable<Cart>);
        }
    }
}
