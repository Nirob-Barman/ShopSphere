
namespace ShopSphere.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }

        public string? Method { get; set; } // e.g., Credit Card, PayPal
        public string? TransactionId { get; set; }
        public string? Status { get; set; } // e.g., Paid, Failed

        public Guid OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
