using ShoppingCart.Shared;
using ShoppingCart.Shared.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart
{
    public class InMemoryProductReposiotry : IQueryableByIdRepository<Product>
    {
        private readonly IDataProvider<Product> dataProvider;
        private IEnumerable<Product> products;

        public InMemoryProductReposiotry(IDataProvider<Product> dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        public async Task<Product> GetById(long id)
        {
            await EnsureData();
            return products.FirstOrDefault(x => x.ID == id);
        }

        public async Task<Product> GetByName(string name)
        {
            await EnsureData();
            return products.FirstOrDefault(x => x.Name == name);
        }

        private async Task EnsureData()
        {
            if(products == null)
            {
                products = await dataProvider.Provide();
            }
        }
    }
}