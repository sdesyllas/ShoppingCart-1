namespace ShoppingCart.Shared.Model
{
    public class Product
    {
        public long ID { get; }
        public string Name { get; }
        public string Description { get; }
        public decimal Price { get; }
        public int Stock { get; set; }

        public Product(long Identifier, string Name, string Description, decimal Price, int Stock)
        {
            ID = Identifier;
            this.Name = Name;
            this.Description = Description;
            this.Price = Price;
            this.Stock = Stock;
        }
    }
}
