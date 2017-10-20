using ShoppingCart.Repository.Exceptions;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Repository
{
    /// <summary>
    /// In memory repository for <see cref="Cart"/>. Provides no data persistance
    /// </summary>
    public class InMemoryCartRepository : ICartRepository
    {
        private readonly IDataProvider<Cart> _dataProvider;
        private IEnumerable<Cart> _baskets;

        /// <summary>
        /// Creates intance of <see cref="InMemoryCartRepository"/>
        /// </summary>
        /// <param name="dataProvider">Provider for intial <see cref="Cart"/> collection</param>
        public InMemoryCartRepository(IDataProvider<Cart> dataProvider)
        {
            _dataProvider = dataProvider;
        }

        /// <summary>
        /// Gets first <see cref="Cart"/> matching a predicate
        /// </summary>
        /// <param name="predicate">Search predicate</param>
        /// <returns>First <see cref="Cart"/> matching predicate</returns>
        /// <exception cref="CartNotFoundException"><see cref="Cart"/> not found</exception>
        public async Task<Cart> GetAsync(Func<Cart, bool> predicate)
        {
            await EnsureDataAsync();
            try
            {
                return _baskets.First(predicate);
            }
            catch (InvalidOperationException e)
            {
                throw new CartNotFoundException(e);
            }
        }

        /// <summary>
        /// Checks out a <see cref="Cart"/>. Reduces products stock
        /// </summary>
        /// <param name="cartName"><see cref="Cart"/> name to check out</param>
        /// <param name="productProvider">Function finding a <see cref="Product"/> by id</param>
        /// <returns>Task representinc async operation</returns>
        /// <exception cref="CartNotFoundException"><see cref="Cart"/> not found</exception>
        /// <exception cref="ProdcutNotFoundException"><see cref="Product"/> not found</exception>
        /// <exception cref="CartCheckedOutException"><see cref="Cart"/> already checked out</exception>
        /// <exception cref="NotEnoughStockException">Insufficient product stock</exception>
        public async Task CheckoutAsync(string cartName, Func<long, Task<Product>> productProvider)
        {
            var cart = await GetAsync(x => x.Name == cartName);
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
        
        private async Task EnsureStockAsync(Cart cart, Func<long, Task<Product>> productProvider)
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

        /// <summary>
        /// Adds item to cart
        /// </summary>
        /// <param name="cartName">Cart name to add item to</param>
        /// <param name="productProvider">Function finding a product by id</param>
        /// <param name="item">Item to add</param>
        /// <returns>Task representinc async operation</returns>
        /// <exception cref="CartNotFoundException"><see cref="Cart"/> not found</exception>
        /// <exception cref="ProdcutNotFoundException"><see cref="Product"/> not found</exception>
        /// <exception cref="CartCheckedOutException"><see cref="Cart"/> already checked out</exception>
        /// <exception cref="NotEnoughStockException">Insufficient product stock</exception>
        public async Task AddItemToCartAsync(string cartName, Func<long, Task<Product>> productProvider, CartItem item)
        {
            var cart = await GetAsync(x => x.Name == cartName);
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
