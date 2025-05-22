using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopSphere.Application.DTOs.Admin;
using ShopSphere.Application.Interfaces;

namespace ShopSphere.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public AdminController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
        {
            var result = await _identityService.CreateRoleAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("delete-role")]
        public async Task<IActionResult> DeleteRole([FromBody] DeleteRoleRequest request)
        {
            var result = await _identityService.DeleteRoleAsync(request);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
        {
            var result = await _identityService.AssignRoleAsync(request);
            return StatusCode(result.StatusCode, result);
        }
        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveRole([FromBody] RemoveRoleRequest request)
        {
            var result = await _identityService.RemoveRoleAsync(request);
            return StatusCode(result.StatusCode, result);
        }
    }
}