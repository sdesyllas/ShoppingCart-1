using ShoppingCart.Shared;
using ShoppingCart.Shared.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart
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
            return _products.FirstOrDefault(x => x.Id == id);
        }

        public async Task<Product> GetByNameAsync(string name)
        {
            await EnsureDataAsync();
            return _products.FirstOrDefault(x => x.Name == name);
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