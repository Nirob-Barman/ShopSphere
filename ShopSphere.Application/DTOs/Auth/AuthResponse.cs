
namespace ShopSphere.Application.DTOs.Auth
{
    public class AuthResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime AccessTokenExpiresAt { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
    }
}
