using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopSphere.Application.DTOs.Admin;
using ShopSphere.Application.DTOs.Auth;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Interfaces.Email;
using ShopSphere.Application.Interfaces.Persistence;
using ShopSphere.Application.Validators.Auth;
using ShopSphere.Application.Wrappers;
using ShopSphere.Domain.Entities;
using ShopSphere.Infrastructure.Identity.Entity;
using ShopSphere.Infrastructure.Persistence;
using System.Net;
using System.Security.Claims;

namespace ShopSphere.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IJwtTokenService jwtTokenService, ApplicationDbContext context, IEmailService emailService, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenService = jwtTokenService;
            _context = context;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request)
        {
            var validationErrors = RegisterRequestValidator.Validate(request);
            if (validationErrors.Any())
            {
                return ApiResponse<RegisterResponse>.ValidationErrorResponse("Validation failed", validationErrors);
            }

            //var existingUser = await _userManager.FindByEmailAsync(request.Email!);
            var existingUser = await FindByEmailAsync(request.Email!);
            if (existingUser != null)
            {
                return ApiResponse<RegisterResponse>.ConflictResponse("Registration failed", new[] { "A user with this email already exists." });
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

                var welcomeMessage = $"Hello {request.FullName},<br>Welcome to ShopSphere! Thank you for registering.";
                await _emailService.SendEmailAsync(user.Email!, "Welcome to ShopSphere", welcomeMessage);

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
                return ApiResponse<AuthResponse>.ValidationErrorResponse("Validation failed", validationErrors);
            }
            //var user = await _userManager.FindByEmailAsync(request.Email!);
            var user = await FindByEmailAsync(request.Email!);
            if (user == null)
            {
                return ApiResponse<AuthResponse>.NotFoundResponse("Email not found");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password!);
            if (!passwordValid)
            {
                return ApiResponse<AuthResponse>.UnauthorizedResponse("Password is incorrect");
            }

            _unitOfWork.BeginTransaction();

            try
            {
                var tokens = await GenerateTokensAsync(user);

                #region Login Audit Logging
                var httpContext = _httpContextAccessor.HttpContext;
                var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
                var userAgent = httpContext?.Request?.Headers["User-Agent"].ToString() ?? "Unknown";

                var loginAudit = new LoginAudit
                {
                    UserId = user.Id,
                    Email = user.Email!,
                    LoginTime = DateTime.UtcNow,
                    IpAddress = ipAddress,
                    UserAgent = userAgent
                };

                _context.LoginAudits.Add(loginAudit);
                //await _context.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                #endregion

                return ApiResponse<AuthResponse>.SuccessResponse(tokens, "User Logged In successfully");
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<ApiResponse<UserProfileResponse>> GetCurrentUserAsync(ClaimsPrincipal userPrincipal)
        {
            var userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return ApiResponse<UserProfileResponse>.UnauthorizedResponse("User ID not found in token.");

            //var user = await _userManager.FindByIdAsync(userId);
            var user = await FindUserByIdAsync(userId);
            if (user == null)
                return ApiResponse<UserProfileResponse>.NotFoundResponse("User not found.");

            var roles = await _userManager.GetRolesAsync(user);

            var profile = new UserProfileResponse
            {
                Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                FullName = user.FullName,
                Roles = roles.ToList()
            };

            return ApiResponse<UserProfileResponse>.SuccessResponse(profile, "User profile retrieved.");
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var errors = IdentityRequestValidator.ValidateRefreshTokenRequest(request);
            if (errors.Any())
                throw new ArgumentException(string.Join(", ", errors));

            var token = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

            if (token == null || token.IsUsed || token.IsRevoked || token.ExpiresAt < DateTime.UtcNow)
                throw new Exception("Invalid refresh token");

            token.IsUsed = true;
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();

            //var user = await _userManager.FindByIdAsync(token.UserId!)
                       //?? throw new Exception("User not found");
            var user = await FindUserByIdAsync(token.UserId!)
                       ?? throw new Exception("User not found");

            return await GenerateTokensAsync(user);
        }

        public async Task<ApiResponse<string>> LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return ApiResponse<string>.ValidationErrorResponse("Validation failed", new[] { "Refresh token must not be empty." });
            }

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
            //await _context.SaveChangesAsync();

            return new AuthResponse
            {
                AccessToken = accessToken,
                AccessTokenExpiresAt = expiresAt,
                RefreshToken = refreshToken,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? "Customer"
            };
        }

        public async Task<ApiResponse<string>> RequestPasswordResetAsync(string email)
        {
            var validationErrors = IdentityRequestValidator.ValidatePasswordResetRequestEmail(email);
            if (validationErrors.Any())
                return ApiResponse<string>.ValidationErrorResponse("Validation failed", validationErrors);

            //var user = await _userManager.FindByEmailAsync(email);
            var user = await FindByEmailAsync(email);
            if (user == null)
                return ApiResponse<string>.NotFoundResponse("User not found");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            //var resetLink = $"https://yourfrontend.com/reset-password?email={WebUtility.UrlEncode(email)}&token={WebUtility.UrlEncode(token)}";


            var request = _httpContextAccessor.HttpContext?.Request;
            var baseUrl = $"{request?.Scheme}://{request?.Host}";
            var resetLink = $"{baseUrl}/reset-password?email={WebUtility.UrlEncode(email)}&token={WebUtility.UrlEncode(token)}";
            var message = $"<p>Hi {user.FullName},</p><p>Click <a href='{resetLink}'>here</a> to reset your password.</p>";

            await _emailService.SendEmailAsync(user.Email!, "Password Reset Request", message);

            return ApiResponse<string>.SuccessResponse(resetLink, "Password reset link has been sent to your email.");
        }

        public async Task<ApiResponse<string>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var validationErrors = IdentityRequestValidator.ValidateResetPasswordRequest(request);
            if (validationErrors.Any())
                return ApiResponse<string>.ValidationErrorResponse("Validation failed", validationErrors);

            //var user = await _userManager.FindByEmailAsync(request.Email!);
            var user = await FindByEmailAsync(request.Email!);
            if (user == null)
                return ApiResponse<string>.NotFoundResponse("User not found");

            //var result = await _userManager.ResetPasswordAsync(user, request.Token!, request.NewPassword!);
            var decodedToken = WebUtility.UrlDecode(request.Token);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken!, request.NewPassword!);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return ApiResponse<string>.BadRequestResponse("Password reset failed", errors);
            }

            return ApiResponse<string>.SuccessResponse("Password has been reset successfully.");
        }

        public async Task<ApiResponse<RoleActionResponse>> CreateRoleAsync(CreateRoleRequest request)
        {
            var validationErrors = IdentityRequestValidator.ValidateCreateRoleRequest(request);
            if (validationErrors.Any())
                return ApiResponse<RoleActionResponse>.ValidationErrorResponse("Validation failed", validationErrors);

            //var roleExists = await _roleManager.RoleExistsAsync(request.RoleName!);
            var roleExists = await RoleExistsAsync(request.RoleName!);
            if (roleExists)
            {
                return ApiResponse<RoleActionResponse>.ConflictResponse("Role creation failed", new[] { "Role already exists." });
            }

            var role = new IdentityRole(request.RoleName!);
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return ApiResponse<RoleActionResponse>.SuccessResponse(new RoleActionResponse { RoleName = request.RoleName! }, "Role created successfully");
            }

            var errors = result.Errors.Select(e => e.Description).ToArray();
            return ApiResponse<RoleActionResponse>.BadRequestResponse("Role creation failed", errors);
        }


        public async Task<ApiResponse<RoleActionResponse>> DeleteRoleAsync(DeleteRoleRequest request)
        {
            var validationErrors = IdentityRequestValidator.ValidateDeleteRoleRequest(request);
            if (validationErrors.Any())
                return ApiResponse<RoleActionResponse>.ValidationErrorResponse("Validation failed", validationErrors);

            if (string.IsNullOrWhiteSpace(request.RoleName))
            {
                return ApiResponse<RoleActionResponse>.ValidationErrorResponse("Validation failed", new[] { "Role name must not be empty." });
            }

            if (request.RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return ApiResponse<RoleActionResponse>.FailResponse("Role deletion blocked", "Cannot delete the 'Admin' role.");
            }

            var role = await _roleManager.FindByNameAsync(request.RoleName);
            if (role == null)
            {
                return ApiResponse<RoleActionResponse>.NotFoundResponse("Role not found");
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(request.RoleName);
            if (usersInRole.Any())
            {
                return ApiResponse<RoleActionResponse>.FailResponse(
                    "Role deletion failed",
                    $"There are {usersInRole.Count} user(s) assigned to this role. Please remove them first."
                );
            }

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToArray();
                return ApiResponse<RoleActionResponse>.BadRequestResponse("Role deletion failed", errors);
            }

            return ApiResponse<RoleActionResponse>.SuccessResponse(new RoleActionResponse { RoleName = request.RoleName }, "Role deleted successfully.");
        }

        public async Task<ApiResponse<RoleAssignmentResponse>> AssignRoleAsync(AssignRoleRequest request)
        {
            var validationErrors = IdentityRequestValidator.ValidateAssignRoleRequest(request);
            if (validationErrors.Any())
                return ApiResponse<RoleAssignmentResponse>.ValidationErrorResponse("Validation failed", validationErrors);

            //var user = await _userManager.FindByIdAsync(request.UserId!);
            var user = await FindUserByIdAsync(request.UserId!);
            if (user == null)
            {
                return ApiResponse<RoleAssignmentResponse>.NotFoundResponse("User not found");
            }

            //var roleExists = await _roleManager.RoleExistsAsync(request.RoleName!);
            var roleExists = await RoleExistsAsync(request.RoleName!);
            if (!roleExists)
            {
                return ApiResponse<RoleAssignmentResponse>.NotFoundResponse("Role not found");
            }

            var result = await _userManager.AddToRoleAsync(user, request.RoleName!);

            if (result.Succeeded)
            {
                var response = new RoleAssignmentResponse
                {
                    UserId = user.Id,
                    RoleName = request.RoleName!
                };
                return ApiResponse<RoleAssignmentResponse>.SuccessResponse(response, $"Role {request.RoleName} assigned to user successfully");
            }

            var errors = result.Errors.Select(e => e.Description).ToArray();
            return ApiResponse<RoleAssignmentResponse>.BadRequestResponse("Role assignment failed", errors);
        }

        public async Task<ApiResponse<RoleRemovalResponse>> RemoveRoleAsync(RemoveRoleRequest request)
        {
            var validationErrors = IdentityRequestValidator.ValidateRemoveRoleRequest(request);
            if (validationErrors.Any())
                return ApiResponse<RoleRemovalResponse>.ValidationErrorResponse("Validation failed", validationErrors);

            //var user = await _userManager.FindByIdAsync(request.UserId!);
            var user = await FindUserByIdAsync(request.UserId!);
            if (user == null)
            {
                return ApiResponse<RoleRemovalResponse>.NotFoundResponse("User not found");
            }

            //var roleExists = await _roleManager.RoleExistsAsync(request.RoleName!);
            var roleExists = await RoleExistsAsync(request.RoleName!);
            if (!roleExists)
            {
                return ApiResponse<RoleRemovalResponse>.NotFoundResponse("Role not found");
            }

            var isInRole = await _userManager.IsInRoleAsync(user, request.RoleName!);
            if (!isInRole)
            {
                return ApiResponse<RoleRemovalResponse>.BadRequestResponse("User is not assigned to this role");
            }

            var result = await _userManager.RemoveFromRoleAsync(user, request.RoleName!);
            if (result.Succeeded)
            {
                var response = new RoleRemovalResponse
                {
                    UserId = user.Id,
                    RoleName = request.RoleName!
                };

                return ApiResponse<RoleRemovalResponse>.SuccessResponse(response, $"Role {request.RoleName} removed from user {user.Email} successfully");
            }

            var errors = result.Errors.Select(e => e.Description).ToArray();
            return ApiResponse<RoleRemovalResponse>.BadRequestResponse("Failed to remove role", errors);
        }

        private async Task<ApplicationUser?> FindByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            return await _userManager.FindByEmailAsync(email);
        }

        private async Task<ApplicationUser?> FindUserByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return null;

            return await _userManager.FindByIdAsync(userId);
        }

        private async Task<bool> RoleExistsAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return false;
            return await _roleManager.RoleExistsAsync(roleName);
        }
    }
}
