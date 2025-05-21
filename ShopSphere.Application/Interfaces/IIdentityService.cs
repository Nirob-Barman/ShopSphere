
using ShopSphere.Application.DTOs.Auth;
using ShopSphere.Application.Wrappers;

namespace ShopSphere.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request);
        Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task<ApiResponse<string>> LogoutAsync(string refreshToken);
    }
}
