
namespace ShopSphere.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? SKU { get; set; }
        public int StockQuantity { get; set; }
        public ICollection<Category>? Categories { get; set; }
        public string? SellerId { get; set; }
    }
}
