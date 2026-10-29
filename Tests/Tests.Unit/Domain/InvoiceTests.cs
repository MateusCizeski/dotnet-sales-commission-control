using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Tests.Unit.Domain
{
    public class InvoiceTests
    {
        // ---------- Creation ----------

        [Fact]
        public void Create_AlsoCreatesThePendingCommissionWithTheSellerRate()
        {
            var invoice = TestData.Invoice(TestData.Seller(rate: 12.5m), total: 1000m);

            Assert.Equal(InvoiceStatus.Pending, invoice.Status);
            Assert.NotNull(invoice.Commission);
            Assert.Equal(invoice.Id, invoice.Commission.InvoiceId);
            Assert.Equal(12.5m, invoice.Commission.Rate);
            Assert.Equal(125m, invoice.Commission.Amount);
            Assert.Equal(CommissionStatus.Pending, invoice.Commission.Status);
        }

        [Fact]
        public void Create_NormalizesTheTaxId()
        {
            var invoice = new Invoice("INV-0001", TestData.Seller(), TestData.Now, "Customer", "11.222.333/0001-81", 10, null, TestData.Now);

            Assert.Equal("11222333000181", invoice.CustomerTaxId);
        }

        [Fact]
        public void Create_WithoutNumber_Throws()
        {
            Assert.Throws<DomainException>(() => new Invoice("", TestData.Seller(), TestData.Now, "Customer", TestData.ValidCpf, 10, null, TestData.Now));
        }

        [Fact]
        public void Create_ZeroTotal_Throws()
        {
            Assert.Throws<DomainException>(() => TestData.Invoice(total: 0));
        }

        [Fact]
        public void Create_EmptyCustomer_Throws()
        {
            Assert.Throws<DomainException>(() => new Invoice("INV-0001", TestData.Seller(), TestData.Now, "", TestData.ValidCpf, 10, null, TestData.Now));
        }

        [Fact]
        public void Create_InvalidTaxId_Throws()
        {
            Assert.Throws<DomainException>(() => new Invoice("INV-0001", TestData.Seller(), TestData.Now, "Customer", "00000000000", 10, null, TestData.Now));
        }

        [Fact]
        public void Create_InactiveSeller_Throws()
        {
            var seller = TestData.Seller();
            seller.Deactivate();

            Assert.Throws<DomainException>(() => TestData.Invoice(seller));
        }

        // ---------- State machine ----------

        [Fact]
        public void Approve_Pending_Approves()
        {
            var invoice = TestData.Invoice();

            invoice.Approve();

            Assert.Equal(InvoiceStatus.Approved, invoice.Status);
        }

        [Fact]
        public void Approve_Cancelled_Throws()
        {
            var invoice = TestData.Invoice();
            invoice.Cancel();

            Assert.Throws<DomainException>(invoice.Approve);
        }

        [Fact]
        public void Approve_Twice_Throws()
        {
            var invoice = TestData.Invoice();
            invoice.Approve();

            Assert.Throws<DomainException>(invoice.Approve);
        }

        [Fact]
        public void Cancel_AlsoCancelsThePendingCommission()
        {
            var invoice = TestData.Invoice();

            invoice.Cancel();

            Assert.Equal(InvoiceStatus.Cancelled, invoice.Status);
            Assert.Equal(CommissionStatus.Cancelled, invoice.Commission.Status);
        }

        [Fact]
        public void Cancel_ApprovedWithUnpaidCommission_IsAllowed()
        {
            var invoice = TestData.Invoice();
            invoice.Approve();

            invoice.Cancel();

            Assert.Equal(InvoiceStatus.Cancelled, invoice.Status);
        }

        [Fact]
        public void Cancel_WhenCommissionIsPaid_Throws()
        {
            var invoice = TestData.Invoice();
            invoice.Approve();
            invoice.Commission.Pay(TestData.Now);

            Assert.Throws<DomainException>(invoice.Cancel);
        }

        [Fact]
        public void Cancel_Twice_Throws()
        {
            var invoice = TestData.Invoice();
            invoice.Cancel();

            Assert.Throws<DomainException>(invoice.Cancel);
        }

        // ---------- Update ----------

        [Fact]
        public void Update_Pending_RecalculatesTheCommissionWithTheNewSellerRate()
        {
            var invoice = TestData.Invoice(TestData.Seller(rate: 10), total: 1000m);
            var newSeller = new Seller("Other Seller", "52998224725", "other@example.com", 5, TestData.Now);

            invoice.Update(newSeller, 2000m, TestData.Now.AddDays(1));

            Assert.Equal(newSeller.Id, invoice.SellerId);
            Assert.Equal(2000m, invoice.TotalAmount);
            Assert.Equal(2000m, invoice.Commission.BaseAmount);
            Assert.Equal(5m, invoice.Commission.Rate);
            Assert.Equal(100m, invoice.Commission.Amount);
            Assert.Equal(TestData.Now.AddDays(1), invoice.Commission.CalculatedAt);
        }

        [Fact]
        public void Update_Approved_Throws()
        {
            var invoice = TestData.Invoice();
            invoice.Approve();

            Assert.Throws<DomainException>(() => invoice.Update(TestData.Seller(), 10, TestData.Now));
        }

        [Fact]
        public void Update_Cancelled_Throws()
        {
            var invoice = TestData.Invoice();
            invoice.Cancel();

            Assert.Throws<DomainException>(() => invoice.Update(TestData.Seller(), 10, TestData.Now));
        }
    }
}
