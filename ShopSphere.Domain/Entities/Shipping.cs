
namespace ShopSphere.Domain.Entities
{
    public class Shipping
    {
        public Guid Id { get; set; }

        public string? Address { get; set; }
        public string? Method { get; set; } // e.g., Standard, Express
        public string? TrackingNumber { get; set; }
        public string? Status { get; set; } // e.g., Processing, In Transit, Delivered

        public Guid OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
