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
    /// In memory repository for <see cref="Product"/>. Provides no data persistance
    /// </summary>
    public class InMemoryProductReposiotry : IRepository<Product>
    {
        private readonly IDataProvider<Product> _dataProvider;
        private IEnumerable<Product> _products;

        /// <summary>
        /// Creates intance of <see cref="InMemoryProductReposiotry"/>
        /// </summary>
        /// <param name="dataProvider">Provider for intial <see cref="Product"/> collection</param>
        public InMemoryProductReposiotry(IDataProvider<Product> dataProvider)
        {
            this._dataProvider = dataProvider;
        }

        /// <summary>
        /// Gets first <see cref="Product"/> matching a predicate
        /// </summary>
        /// <param name="predicate">Search predicate</param>
        /// <returns>First <see cref="Product"/> matching predicate</returns>
        /// <exception cref="ProdcutNotFoundException"><see cref="Product"/> not found</exception>
        public async Task<Product> GetAsync(Func<Product, bool> predicate)
        {
            await EnsureDataAsync();
            try
            {
                return _products.First(predicate);
            }
            catch (InvalidOperationException e)
            {
                throw new ProdcutNotFoundException(e);
            }
        }

        private async Task EnsureDataAsync()
        {
            if(_products == null)
            {
                _products = await _dataProvider.ProvideAsync();
            }
        }
    }
}