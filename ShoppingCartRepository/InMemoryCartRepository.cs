using ShoppingCart.Shared;
using ShoppingCart.Shared.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Repository
{
    public class InMemoryCartRepository : IRepository<Cart>
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
            return _baskets.FirstOrDefault(x => x.Name == name);
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
