using Domain.Entities;

namespace Tests.Unit
{
    /// <summary>Fictitious data shared by the unit tests.</summary>
    public static class TestData
    {
        public const string ValidCpf = "12345678909";
        public const string ValidCnpj = "11222333000181";
        public static readonly DateTime Now = new(2026, 10, 1, 12, 0, 0, DateTimeKind.Utc);

        public static Seller Seller(decimal rate = 10) => new("Test Seller", ValidCpf, "seller@example.com", rate, Now);

        public static Invoice Invoice(Seller? seller = null, decimal total = 1000m) =>
            new("INV-0001", seller ?? Seller(), Now.Date, "Customer Ltd", ValidCnpj, total, null, Now);
    }

    /// <summary>A clock that always returns the same instant, so dates in tests are predictable.</summary>
    public class FixedTimeProvider : TimeProvider
    {
        private readonly DateTimeOffset _now;

        public FixedTimeProvider(DateTime utcNow)
        {
            _now = new DateTimeOffset(utcNow, TimeSpan.Zero);
        }

        public override DateTimeOffset GetUtcNow() => _now;
    }
}
