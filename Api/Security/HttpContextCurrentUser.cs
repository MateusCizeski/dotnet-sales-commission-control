using Application.Abstractions;
using Domain.Enums;
using System.Security.Claims;

namespace Api.Security
{
    /// <summary>Reads the current user from the validated JWT claims.</summary>
    public class HttpContextCurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpContextCurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

        public Guid? UserId => ParseGuid(Principal?.FindFirstValue(TokenClaims.Subject));

        public UserRole? Role => Enum.TryParse<UserRole>(Principal?.FindFirstValue(TokenClaims.Role), out var role) ? role : null;

        public Guid? SellerId => ParseGuid(Principal?.FindFirstValue(TokenClaims.SellerId));

        private static Guid? ParseGuid(string? value) => Guid.TryParse(value, out var id) ? id : null;
    }
}
