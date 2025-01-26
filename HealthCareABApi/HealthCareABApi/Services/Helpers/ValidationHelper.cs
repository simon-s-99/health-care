using System.Text.RegularExpressions;

namespace HealthCareABApi.Services.Helpers
{
    public class ValidationHelper
    {
        public static bool HasComplexity(string password)
        {
            return password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit) &&
                   password.Any(ch => !char.IsLetterOrDigit(ch)); // Special character check
        }

        public static bool IsValidEmail(string email)
        {
            var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // Basic email format validation. Not pretty but should work!
            return Regex.IsMatch(email, emailRegex, RegexOptions.IgnoreCase);
        }

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            var phoneRegex = @"^\+?[1-9]\d{1,14}$"; // E.164 format validation
            return Regex.IsMatch(phoneNumber, phoneRegex);
        }
    }
}
