using ShopSphere.Application.DTOs.Admin;
using ShopSphere.Application.DTOs.Auth;
using ShopSphere.Application.Validators.Common;

namespace ShopSphere.Application.Validators.Auth
{
    public static class IdentityRequestValidator
    {
        public static List<string> ValidateResetPasswordRequest(ResetPasswordRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Email))
                errors.Add("Email is required.");
            else if (!EmailValidator.IsValid(request.Email))
                errors.Add("Invalid email format.");

            if (string.IsNullOrWhiteSpace(request.Token))
                errors.Add("Reset token is required.");

            if (string.IsNullOrWhiteSpace(request.NewPassword))
                errors.Add("New password is required.");
            else if (request.NewPassword.Length < 6)
                errors.Add("Password must be at least 6 characters long.");

            return errors;
        }
        public static List<string> ValidateCreateRoleRequest(CreateRoleRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.RoleName))
                errors.Add("Role name is required.");

            return errors;
        }

        public static List<string> ValidateAssignRoleRequest(AssignRoleRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.UserId))
                errors.Add("User ID is required.");

            if (string.IsNullOrWhiteSpace(request.RoleName))
                errors.Add("Role name is required.");

            return errors;
        }

        public static List<string> ValidateRemoveRoleRequest(RemoveRoleRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.UserId))
                errors.Add("User ID is required.");

            if (string.IsNullOrWhiteSpace(request.RoleName))
                errors.Add("Role name is required.");

            return errors;
        }

        public static List<string> ValidateDeleteRoleRequest(DeleteRoleRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.RoleName))
                errors.Add("Role name is required.");

            return errors;
        }

        public static List<string> ValidatePasswordResetRequestEmail(string email)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(email))
                errors.Add("Email is required.");
            else if (!EmailValidator.IsValid(email))
                errors.Add("Invalid email format.");

            return errors;
        }

        public static List<string> ValidateRefreshTokenRequest(RefreshTokenRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                errors.Add("Refresh token is required.");

            return errors;
        }
    }
}
