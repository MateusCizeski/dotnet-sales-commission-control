using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using IPasswordHasher = Application.Abstractions.IPasswordHasher;

namespace Infra.Security
{
    /// <summary>
    /// Adapter over ASP.NET Core Identity's PasswordHasher (PBKDF2 with HMAC-SHA512, salt and
    /// iteration count stored in the hash). Proven library code instead of a hand-written hash.
    /// </summary>
    public class IdentityPasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<User> _hasher = new();

        public string Hash(User user, string password)
        {
            return _hasher.HashPassword(user, password);
        }

        public bool Verify(User user, string password)
        {
            try
            {
                var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
                return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
            }
            catch (FormatException)
            {
                // Corrupted hash in the database: treat as a failed login, never as success.
                return false;
            }
        }
    }
}
