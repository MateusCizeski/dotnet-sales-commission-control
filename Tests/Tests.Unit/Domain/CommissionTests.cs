using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Tests.Unit.Domain
{
    public class CommissionTests
    {
        [Theory]
        [InlineData(1000, 12.5, 125)]       // 12.5% of 1000
        [InlineData(999.99, 10, 100)]       // 99.999 → 100.00
        [InlineData(1234.56, 7.5, 92.59)]   // 92.592 → 92.59
        [InlineData(1000, 5, 50)]
        [InlineData(1000, 15, 150)]
        [InlineData(2000, 12.5, 250)]
        public void Calculate_RoundsToTwoDecimals(decimal baseAmount, decimal rate, decimal expected)
        {
            Assert.Equal(expected, Commission.Calculate(baseAmount, rate));
        }

        [Fact]
        public void Pay_Pending_SetsStatusAndDate()
        {
            var commission = TestData.Invoice().Commission;

            commission.Pay(TestData.Now);

            Assert.Equal(CommissionStatus.Paid, commission.Status);
            Assert.Equal(TestData.Now, commission.PaidAt);
        }

        [Fact]
        public void Pay_Twice_Throws()
        {
            var commission = TestData.Invoice().Commission;
            commission.Pay(TestData.Now);

            Assert.Throws<DomainException>(() => commission.Pay(TestData.Now));
        }

        [Fact]
        public void Pay_Cancelled_Throws()
        {
            var commission = TestData.Invoice().Commission;
            commission.Cancel();

            Assert.Throws<DomainException>(() => commission.Pay(TestData.Now));
        }

        [Fact]
        public void Cancel_Paid_Throws()
        {
            var commission = TestData.Invoice().Commission;
            commission.Pay(TestData.Now);

            Assert.Throws<DomainException>(commission.Cancel);
        }

        [Fact]
        public void Cancel_Twice_Throws()
        {
            var commission = TestData.Invoice().Commission;
            commission.Cancel();

            Assert.Throws<DomainException>(commission.Cancel);
        }
    }
}
