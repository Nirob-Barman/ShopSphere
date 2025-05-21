using ShopSphere.Application.DTOs.Auth;

namespace ShopSphere.Application.Validators.Auth
{
    public static class LoginRequestValidator
    {
        public static List<string> Validate(LoginRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Email))
                errors.Add("Email is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                errors.Add("Password is required.");

            // You can also add format checks here, e.g. for email pattern

            return errors;
        }
    }
}
