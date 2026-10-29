using Application.Abstractions;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Api.Security
{
    /// <summary>Claim names written in the token and read by <see cref="HttpContextCurrentUser"/>.</summary>
    public static class TokenClaims
    {
        public const string Subject = "sub";
        public const string Email = "email";
        public const string Role = "role";
        public const string SellerId = "seller_id";
    }

    /// <summary>"Jwt" configuration section. The secret comes from User Secrets / environment variables.</summary>
    public class JwtOptions
    {
        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpirationMinutes { get; set; } = 60;
    }

    /// <summary>Issues HS256 access tokens. There is no refresh token: the client logs in again after expiration.</summary>
    public class JwtTokenService : ITokenService
    {
        private readonly JwtOptions _options;
        private readonly TimeProvider _timeProvider;
        private readonly JsonWebTokenHandler _handler = new();

        public JwtTokenService(IOptions<JwtOptions> options, TimeProvider timeProvider)
        {
            _options = options.Value;
            _timeProvider = timeProvider;
        }

        public AccessToken CreateToken(User user)
        {
            DateTime now = _timeProvider.GetUtcNow().UtcDateTime;
            DateTime expiresAt = now.AddMinutes(_options.ExpirationMinutes);

            var claims = new Dictionary<string, object>
            {
                [TokenClaims.Subject] = user.Id.ToString(),
                [TokenClaims.Email] = user.Email,
                [TokenClaims.Role] = user.Role.ToString()
            };

            if (user.SellerId.HasValue)
            {
                claims[TokenClaims.SellerId] = user.SellerId.Value.ToString();
            }

            var descriptor = new SecurityTokenDescriptor
            {
                Issuer = _options.Issuer,
                Audience = _options.Audience,
                IssuedAt = now,
                NotBefore = now,
                Expires = expiresAt,
                Claims = claims,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret)),
                    SecurityAlgorithms.HmacSha256)
            };

            return new AccessToken(_handler.CreateToken(descriptor), expiresAt);
        }
    }
}
