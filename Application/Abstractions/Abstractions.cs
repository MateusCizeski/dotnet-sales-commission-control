using Domain.Entities;
using Domain.Enums;

namespace Application.Abstractions
{
    // Ports implemented by the outer layers (Api / Infra). The Application layer only knows these interfaces.

    /// <summary>The authenticated user of the current request (read from the JWT by the Api layer).</summary>
    public interface ICurrentUser
    {
        Guid? UserId { get; }
        UserRole? Role { get; }

        /// <summary>Seller linked to the user when the role is Seller; null for Admin.</summary>
        Guid? SellerId { get; }

        bool IsAdmin => Role == UserRole.Admin;
    }

    /// <summary>Generates the next invoice number (INV-0001). Implemented with a database SEQUENCE.</summary>
    public interface IInvoiceNumberGenerator
    {
        Task<string> NextAsync(CancellationToken cancellationToken = default);
    }

    public interface IPasswordHasher
    {
        string Hash(User user, string password);
        bool Verify(User user, string password);
    }

    public record AccessToken(string Token, DateTime ExpiresAt);

    public interface ITokenService
    {
        AccessToken CreateToken(User user);
    }
}
