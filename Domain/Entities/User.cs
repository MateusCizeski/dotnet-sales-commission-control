using ApiBase.Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Validation;

namespace Domain.Entities
{
    /// <summary>
    /// Login account. A Seller user is linked to one seller record and can only read that seller's data.
    /// Only the password hash is stored.
    /// </summary>
    public class User : EntityGuid
    {
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public UserRole Role { get; private set; }
        public Guid? SellerId { get; private set; }

        protected User() { }

        private User(string email, string passwordHash, UserRole role, Guid? sellerId)
        {
            if (string.IsNullOrWhiteSpace(email) || !EmailValidator.EmailIsValid(email))
            {
                throw new DomainException("Email is invalid.");
            }

            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                throw new DomainException("Password hash is required.");
            }

            Email = email.Trim().ToLowerInvariant();
            PasswordHash = passwordHash;
            Role = role;
            SellerId = sellerId;
        }

        public static User CreateAdmin(string email, string passwordHash)
        {
            return new User(email, passwordHash, UserRole.Admin, null);
        }

        public static User CreateSeller(string email, string passwordHash, Seller seller)
        {
            return new User(email, passwordHash, UserRole.Seller, seller.Id);
        }
    }
}
