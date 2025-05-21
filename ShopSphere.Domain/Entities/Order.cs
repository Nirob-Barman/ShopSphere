
namespace ShopSphere.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        public string? UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; } // e.g., Pending, Shipped, Delivered
        public DateTime CreatedAt { get; set; }
    }
}
