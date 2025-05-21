
using ShopSphere.Application.DTOs.Auth;

namespace ShopSphere.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(JwtUserInfo user, IList<string> roles, out DateTime expiresAt);
        string GenerateRefreshToken();
    }
}
