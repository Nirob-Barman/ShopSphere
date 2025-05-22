
namespace ShopSphere.Application.DTOs.Products
{
    public class CreateProductRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? SKU { get; set; }
        public int StockQuantity { get; set; }
        //public List<Guid>? CategoryIds { get; set; }
        public string? SellerId { get; set; }
    }
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? SKU { get; set; }
        public int StockQuantity { get; set; }
        public List<string>? Categories { get; set; }
        public string? SellerId { get; set; }
    }
    public class UpdateProductRequest
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? SKU { get; set; }
        public int StockQuantity { get; set; }
        //public List<Guid>? CategoryIds { get; set; }
        public string? SellerId { get; set; }
    }
}
