using System.Text.RegularExpressions;

namespace ShopSphere.Application.Validators.Common
{
    public static class EmailValidator
    {
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool IsValid(string email)
        {
            return !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);
        }
    }
}
