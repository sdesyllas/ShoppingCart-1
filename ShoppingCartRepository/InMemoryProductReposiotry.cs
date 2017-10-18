using ShoppingCart.Repository.Exceptions;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Repository
{
    public class InMemoryProductReposiotry : IQueryableByIdRepository<Product>
    {
        private readonly IDataProvider<Product> _dataProvider;
        private IEnumerable<Product> _products;

        public InMemoryProductReposiotry(IDataProvider<Product> dataProvider)
        {
            this._dataProvider = dataProvider;
        }

        public async Task<Product> GetByIdAsync(long id)
        {
            await EnsureDataAsync();
            try
            {
                return _products.First(x => x.Id == id);
            }
            catch (InvalidOperationException e)
            {
                throw new ProdcutNotFoundException(e);
            }
        }

        public async Task<Product> GetByNameAsync(string name)
        {
            await EnsureDataAsync();
            try
            {
                return _products.First(x => x.Name == name);
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