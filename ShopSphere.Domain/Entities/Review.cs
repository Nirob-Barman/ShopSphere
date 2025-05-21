
namespace ShopSphere.Domain.Entities
{
    public class Review
    {
        public Guid Id { get; set; }

        public string? UserId { get; set; }

        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        public int Rating { get; set; } // 1-5
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
