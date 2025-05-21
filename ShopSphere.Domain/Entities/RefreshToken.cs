
namespace ShopSphere.Infrastructure.Identity.Entity
{
    public class RefreshToken
    {
        public string? Token { get; set; }
        public string? UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsUsed { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
