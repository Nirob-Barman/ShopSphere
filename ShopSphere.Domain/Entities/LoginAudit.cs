
namespace ShopSphere.Domain.Entities
{
    public class LoginAudit
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? Email { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }

}
