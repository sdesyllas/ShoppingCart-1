using ShoppingCart.Shared;
using ShoppingCart.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Repository
{
    public class InMemoryCartRepository : ICartRepository
    {
        private readonly IDataProvider<Cart> _dataProvider;
        private IEnumerable<Cart> _baskets;

        public InMemoryCartRepository(IDataProvider<Cart> dataProvider)
        {
            this._dataProvider = dataProvider;
        }

        public async Task<Cart> GetByNameAsync(string name)
        {
            await EnsureDataAsync();
            try
            {
                return _baskets.First(x => x.Name == name);
            }
            catch (InvalidOperationException e)
            {
                throw new CartNotFoundException(e);
            }
        }

        public async Task CheckoutAsync(string cartName, Func<long, Task<Product>> productProvider)
        {
            var cart = await GetByNameAsync(cartName);
            foreach (var item in cart.Items)
            {
                var product = await productProvider(item.ProductId);
                product.Stock -= item.Quantity;
            }

            cart.IsCheckedOut = true;
        }

        private async Task EnsureDataAsync()
        {
            if (_baskets == null)
            {
                _baskets = await _dataProvider.ProvideAsync();
            }
        }
    }
}
