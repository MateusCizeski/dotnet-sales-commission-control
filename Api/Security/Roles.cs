using Domain.Enums;

namespace Api.Security
{
    public static class Roles
    {
        public const string Admin = nameof(UserRole.Admin);
        public const string AdminOrSeller = nameof(UserRole.Admin) + "," + nameof(UserRole.Seller);
    }

    public static class RateLimitPolicies
    {
        public const string Login = "login";
    }
}
