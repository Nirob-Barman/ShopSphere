using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopSphere.Application.DTOs.Auth;
using ShopSphere.Application.Interfaces;

namespace ShopSphere.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _identityService.RegisterAsync(request);
            return StatusCode(result.StatusCode, result);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _identityService.LoginAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        //[Authorize]
        [AllowAnonymous]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var result = await _identityService.GetCurrentUserAsync(User);
            return StatusCode(result.StatusCode, result);
        }


        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var result = await _identityService.RefreshTokenAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var restult = await _identityService.LogoutAsync(request.RefreshToken!);
            return StatusCode(restult.StatusCode, restult);
        }


        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequest request)
        {
            var result = await _identityService.RequestPasswordResetAsync(request.Email!);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _identityService.ResetPasswordAsync(request);
            return StatusCode(result.StatusCode, result);
        }


    }
}
