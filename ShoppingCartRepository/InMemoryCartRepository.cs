using ShoppingCart.Shared;
using ShoppingCart.Shared.Model;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart.Repository
{
    public class InMemoryCartRepository : IRepository<Cart>
    {
        private readonly List<Cart> baskets;

        public InMemoryCartRepository(IDataProvider<Cart> dataProvider)
        {
            baskets = dataProvider.Provide();
        }

        public Cart GetByName(string name)
        {
            return baskets.FirstOrDefault(x => x.Name == name);
        }
    }
}
