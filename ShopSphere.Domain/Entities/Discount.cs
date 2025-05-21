
namespace ShopSphere.Domain.Entities
{
    public class Discount
    {
        public Guid Id { get; set; }

        public string? Code { get; set; }
        public decimal Percentage { get; set; }
        public int UsageLimit { get; set; }
        public DateTime ExpiryDate { get; set; }

        public ICollection<Product>? ApplicableProducts { get; set; }
        public ICollection<Category>? ApplicableCategories { get; set; }
    }
}
