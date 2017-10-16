using ShoppingCart.Shared;
using ShoppingCart.Shared.Model;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart
{
    public class InMemoryProductReposiotry : IQueryableByIdRepository<Product>
    {
        private IEnumerable<Product> products;

        public InMemoryProductReposiotry(IDataProvider<Product> productsProvider)
        {
            products = productsProvider.Provide();
        }

        public Product GetById(long id)
        {
            return products.FirstOrDefault(x => x.ID == id);
        }

        public Product GetByName(string name)
        {
            return products.FirstOrDefault(x => x.Name == name);
        }
    }
}