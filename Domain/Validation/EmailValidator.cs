using System.Text.RegularExpressions;

namespace Domain.Validation
{
    public static class EmailValidator
    {
        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public static bool EmailIsValid(string email)
        {
            return EmailRegex.IsMatch(email);
        }
    }
}
