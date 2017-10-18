using ShoppingCart.Repository.Exceptions;
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
            _dataProvider = dataProvider;
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
            if (cart.IsCheckedOut)
            {
                throw new CartCheckedOutException();
            }

            await EnsureStockAsync(cart, productProvider);

            foreach (var item in cart.Items)
            {
                var product = await productProvider(item.ProductId);
                product.Stock -= item.Quantity;
            }

            cart.IsCheckedOut = true;
        }

        public async Task EnsureStockAsync(Cart cart, Func<long, Task<Product>> productProvider)
        {
            var productTasks = cart.Items
                .GroupBy(x => x.ProductId)
                .Select(async x => new
                {
                    Id = x.Key,
                    CartSum = x.Sum(y => y.Quantity),
                    Product = await productProvider(x.Key)
                });
            bool anyItemBelowStock = (await Task.WhenAll(productTasks)).Any(x => x.Product.Stock < x.CartSum);
            if (anyItemBelowStock)
            {
                throw new NotEnoughStockException();
            }
        }

        private async Task<IEnumerable<Product>> GetProductsFromCartItemsAsync(IEnumerable<CartItem> items, Func<long, Task<Product>> productProvider)
        {
            var productTasks = items
                .Select(x => x.ProductId)
                .Distinct()
                .Select(productProvider);
            return await Task.WhenAll(productTasks);
        }

        public async Task AddItemToCartAsync(string cartName, Func<long, Task<Product>> productProvider, CartItem item)
        {
            var cart = await GetByNameAsync(cartName);
            if (cart.IsCheckedOut)
            {
                throw new CartCheckedOutException();
            }

            var product = await productProvider(item.ProductId);
            if (product.Stock < item.Quantity)
            {
                throw new NotEnoughStockException();
            }

            cart.Items.Add(item);
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
