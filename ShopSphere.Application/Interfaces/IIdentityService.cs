using ShopSphere.Application.DTOs.Admin;
using ShopSphere.Application.DTOs.Auth;
using ShopSphere.Application.Wrappers;
using System.Security.Claims;

namespace ShopSphere.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request);
        Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
        Task<ApiResponse<UserProfileResponse>> GetCurrentUserAsync(ClaimsPrincipal user);
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task<ApiResponse<string>> LogoutAsync(string refreshToken);
        Task<ApiResponse<string>> RequestPasswordResetAsync(string email);
        Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordRequest request);
        Task<ApiResponse<RoleActionResponse>> CreateRoleAsync(CreateRoleRequest request);
        Task<ApiResponse<RoleActionResponse>> DeleteRoleAsync(DeleteRoleRequest request);
        Task<ApiResponse<RoleAssignmentResponse>> AssignRoleAsync(AssignRoleRequest request);
        Task<ApiResponse<RoleRemovalResponse>> RemoveRoleAsync(RemoveRoleRequest request);
    }
}
