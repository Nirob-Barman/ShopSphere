
using Microsoft.AspNetCore.Identity;

namespace ShopSphere.Infrastructure.Identity.Entity
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
