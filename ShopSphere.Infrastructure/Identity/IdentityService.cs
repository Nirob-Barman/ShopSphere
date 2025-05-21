
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopSphere.Application.DTOs.Auth;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Validators.Auth;
using ShopSphere.Application.Wrappers;
using ShopSphere.Infrastructure.Identity.Entity;
using ShopSphere.Infrastructure.Persistence;

namespace ShopSphere.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ApplicationDbContext _context;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenService jwtTokenService,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
            _context = context;
        }

        public async Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            var validationErrors = RegisterRequestValidator.Validate(request);
            if (validationErrors.Any())
            {
                return ApiResponse<RegisterResponse>.BadRequestResponse("Validation failed", validationErrors);
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email!);
            if (existingUser != null)
            {
                return ApiResponse<RegisterResponse>.BadRequestResponse("Registration failed", new[] { "A user with this email already exists." });
            }

            // Start a transaction
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = new ApplicationUser
                {
                    Email = request.Email,
                    UserName = request.Email,
                    FullName = request.FullName
                };

                var result = await _userManager.CreateAsync(user, request.Password!);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToArray();
                    return ApiResponse<RegisterResponse>.BadRequestResponse("Registration failed", errors);
                }

                await _userManager.AddToRoleAsync(user, request.Role);
                await transaction.CommitAsync();
                var registerResponse = new RegisterResponse(user.Email!, user.FullName!, request.Role);
                return ApiResponse<RegisterResponse>.CreatedResponse(registerResponse, "User registered successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResponse<RegisterResponse>.FailResponse("An error occurred during registration.", ex.Message);
            }

        }

        public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var validationErrors = LoginRequestValidator.Validate(request);
            if (validationErrors.Any())
            {
                return ApiResponse<AuthResponse>.BadRequestResponse("Validation failed", validationErrors);
            }
            var user = await _userManager.FindByEmailAsync(request.Email!);
            if (user == null)
            {
                return ApiResponse<AuthResponse>.NotFoundResponse("Email not found");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password!);
            if (!passwordValid)
            {
                return ApiResponse<AuthResponse>.UnauthorizedResponse("Password is incorrect");
            }

            var tokens = await GenerateTokensAsync(user);
            return ApiResponse<AuthResponse>.SuccessResponse(tokens, "User Logged In successfully");
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

            if (token == null || token.IsUsed || token.IsRevoked || token.ExpiresAt < DateTime.UtcNow)
                throw new Exception("Invalid refresh token");

            token.IsUsed = true;
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(token.UserId!)
                       ?? throw new Exception("User not found");

            return await GenerateTokensAsync(user);
        }

        public async Task<ApiResponse<string>> LogoutAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
            if (token == null)
                return ApiResponse<string>.NotFoundResponse("Refresh token not found");

            token.IsRevoked = true;
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("Logout successful");
        }

        private async Task<AuthResponse> GenerateTokensAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var jwtUser = new JwtUserInfo
            {
                Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName!
            };

            var accessToken = _jwtTokenService.GenerateAccessToken(jwtUser, roles, out var expiresAt);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = expiresAt,
                RefreshToken = refreshToken,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "Customer"
            };
        }
    }
}
