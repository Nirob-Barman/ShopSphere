
namespace ShopSphere.Application.DTOs.Auth
{
    public class RegisterResponse
    {
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; }

        public RegisterResponse(string email, string fullName, string role)
        {
            Email = email;
            FullName = fullName;
            Role = role;
        }
    }
}
