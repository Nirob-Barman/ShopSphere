
namespace ShopSphere.Application.DTOs.Auth
{
    public class UserProfileResponse
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public List<string>? Roles { get; set; }
    }
}
