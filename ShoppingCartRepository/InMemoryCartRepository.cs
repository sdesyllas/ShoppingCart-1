using ShoppingCart.Shared;
using ShoppingCart.Shared.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Repository
{
    public class InMemoryCartRepository : IRepository<Cart>
    {
        private readonly IDataProvider<Cart> dataProvider;
        private IEnumerable<Cart> baskets;

        public InMemoryCartRepository(IDataProvider<Cart> dataProvider)
        {
            this.dataProvider = dataProvider;
        }

        public async Task<Cart> GetByName(string name)
        {
            await EnsureData();
            return baskets.FirstOrDefault(x => x.Name == name);
        }

        private async Task EnsureData()
        {
            if (baskets == null)
            {
                baskets = await dataProvider.Provide();
            }
        }
    }
}
