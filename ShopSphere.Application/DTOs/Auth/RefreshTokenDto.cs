
namespace ShopSphere.Application.DTOs.Auth
{
    public class RefreshTokenDto
    {
        public string? Token { get; set; }
        public string? UserId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
