
namespace ShopSphere.Application.DTOs.Admin
{
    public class CreateRoleRequest
    {
        public string? RoleName { get; set; }
    }
    public class RoleActionResponse
    {
        public string? RoleName { get; set; }
    }
    public class DeleteRoleRequest
    {
        public string? RoleName { get; set; }
    }
    public class AssignRoleRequest
    {
        public string? UserId { get; set; }
        public string? RoleName { get; set; }
    }
    public class RoleAssignmentResponse
    {
        public string? UserId { get; set; }
        public string? RoleName { get; set; }
    }
    public class RemoveRoleRequest
    {
        public string? UserId { get; set; }
        public string? RoleName { get; set; }
    }
    public class RoleRemovalResponse
    {
        public string? UserId { get; set; }
        public string? RoleName { get; set; }
    }
}
