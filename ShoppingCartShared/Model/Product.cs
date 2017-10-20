namespace ShoppingCart.Shared.Model
{
    /// <summary>
    /// Model for a product
    /// </summary>
    public class Product
    {
        public long Id { get; }
        public string Name { get; }
        public string Description { get; }
        public decimal Price { get; }
        public int Stock { get; set; }

        public Product(long Identifier, string Name, string Description, decimal Price, int Stock)
        {
            Id = Identifier;
            this.Name = Name;
            this.Description = Description;
            this.Price = Price;
            this.Stock = Stock;
        }
    }
}
