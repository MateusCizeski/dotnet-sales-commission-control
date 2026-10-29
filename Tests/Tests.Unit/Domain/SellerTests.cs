using Domain.Entities;
using Domain.Exceptions;
using Domain.Validation;

namespace Tests.Unit.Domain
{
    public class SellerTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Create_InvalidName_Throws(string? name)
        {
            var ex = Assert.Throws<DomainException>(() => new Seller(name!, TestData.ValidCpf, "seller@example.com", 10, TestData.Now));
            Assert.Equal("Full name is required.", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("11111111111")]
        public void Create_InvalidCpf_Throws(string? cpf)
        {
            var ex = Assert.Throws<DomainException>(() => new Seller("Name", cpf!, "seller@example.com", 10, TestData.Now));
            Assert.Equal("CPF is invalid.", ex.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("not-an-email")]
        public void Create_InvalidEmail_Throws(string? email)
        {
            var ex = Assert.Throws<DomainException>(() => new Seller("Name", TestData.ValidCpf, email!, 10, TestData.Now));
            Assert.Equal("Email is invalid.", ex.Message);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(15.01)]
        public void Create_RateOutOfRange_Throws(decimal rate)
        {
            var ex = Assert.Throws<DomainException>(() => new Seller("Name", TestData.ValidCpf, "seller@example.com", rate, TestData.Now));
            Assert.Equal("Commission rate must be between 0% and 15%.", ex.Message);
        }

        [Fact]
        public void Create_Valid_NormalizesCpfAndStartsActive()
        {
            var seller = new Seller("Test Seller", "123.456.789-09", "seller@example.com", 10, TestData.Now);

            Assert.Equal("12345678909", seller.Cpf);
            Assert.True(seller.IsActive);
            Assert.Equal(TestData.Now, seller.CreatedAt);
            Assert.NotEqual(Guid.Empty, seller.Id);
        }

        [Fact]
        public void UpdateDetails_Valid_Updates()
        {
            var seller = TestData.Seller();

            seller.UpdateDetails("New Name", 12);

            Assert.Equal("New Name", seller.FullName);
            Assert.Equal(12, seller.CommissionRate);
        }

        [Fact]
        public void DeactivateAndActivate_ChangeIsActive()
        {
            var seller = TestData.Seller();

            seller.Deactivate();
            Assert.False(seller.IsActive);
            Assert.Throws<DomainException>(seller.EnsureIsActive);

            seller.Activate();
            Assert.True(seller.IsActive);
        }

        [Theory]
        [InlineData("12345678909", true)]
        [InlineData("11111111111", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void CpfValidator_Works(string? cpf, bool expected)
        {
            Assert.Equal(expected, CpfValidator.CpfIsValid(cpf));
        }

        [Theory]
        [InlineData("someone@example.com", true)]
        [InlineData("not-an-email", false)]
        [InlineData("", false)]
        public void EmailValidator_Works(string email, bool expected)
        {
            Assert.Equal(expected, EmailValidator.EmailIsValid(email));
        }
    }
}
