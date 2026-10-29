using ApiBase.Domain.Exceptions;
using ApiBase.Domain.Interfaces;
using Application.Exceptions;
using Application.Requests;
using Application.Services;
using Domain.Entities;
using Domain.Interfaces;
using Moq;

namespace Tests.Unit.Application
{
    public class SellerServiceTests
    {
        private readonly Mock<ISellerRepository> _sellers = new();
        private readonly Mock<ICommissionRepository> _commissions = new();
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly SellerService _service;

        public SellerServiceTests()
        {
            _service = new SellerService(_sellers.Object, _commissions.Object, _unitOfWork.Object, new FixedTimeProvider(TestData.Now));
        }

        private static CreateSellerRequest CreateRequest() => new()
        {
            FullName = "Test Seller",
            Cpf = "123.456.789-09",
            Email = "seller@example.com",
            CommissionRate = 10
        };

        private Seller ExistingSeller()
        {
            var seller = TestData.Seller();
            _sellers.Setup(r => r.GetByIdAsync(seller.Id, It.IsAny<CancellationToken>())).ReturnsAsync(seller);
            return seller;
        }

        [Fact]
        public async Task Create_Valid_AddsAndCommits()
        {
            var id = await _service.CreateAsync(CreateRequest());

            Assert.NotEqual(Guid.Empty, id);
            _sellers.Verify(r => r.AddAsync(It.Is<Seller>(s => s.Cpf == "12345678909" && s.CreatedAt == TestData.Now), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_DuplicatedCpf_ThrowsBusinessRule()
        {
            // The check uses the normalized CPF (digits only), the way it is stored.
            _sellers.Setup(r => r.CpfExistsAsync("12345678909", It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsAsync<BusinessRuleException>(() => _service.CreateAsync(CreateRequest()));
            _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Create_DuplicatedEmail_ThrowsBusinessRule()
        {
            _sellers.Setup(r => r.EmailExistsAsync("seller@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsAsync<BusinessRuleException>(() => _service.CreateAsync(CreateRequest()));
        }

        [Fact]
        public async Task Update_Existing_UpdatesAndCommits()
        {
            var seller = ExistingSeller();

            await _service.UpdateAsync(seller.Id, new UpdateSellerRequest { FullName = "New Name", CommissionRate = 12 });

            Assert.Equal("New Name", seller.FullName);
            Assert.Equal(12, seller.CommissionRate);
            _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_Unknown_ThrowsNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(Guid.NewGuid(), new UpdateSellerRequest { FullName = "Name" }));
        }

        [Fact]
        public async Task Deactivate_WithCommissions_ThrowsBusinessRule()
        {
            var seller = ExistingSeller();
            _commissions.Setup(c => c.ExistsForSellerAsync(seller.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsAsync<BusinessRuleException>(() => _service.DeactivateAsync(seller.Id));
            Assert.True(seller.IsActive);
        }

        [Fact]
        public async Task Deactivate_WithoutCommissions_Deactivates()
        {
            var seller = ExistingSeller();

            await _service.DeactivateAsync(seller.Id);

            Assert.False(seller.IsActive);
            _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Activate_Activates()
        {
            var seller = ExistingSeller();
            seller.Deactivate();

            await _service.ActivateAsync(seller.Id);

            Assert.True(seller.IsActive);
        }

        [Fact]
        public async Task Delete_WithCommissions_ThrowsBusinessRule()
        {
            var seller = ExistingSeller();
            _commissions.Setup(c => c.ExistsForSellerAsync(seller.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            await Assert.ThrowsAsync<BusinessRuleException>(() => _service.DeleteAsync(seller.Id));
            _sellers.Verify(r => r.Remove(It.IsAny<Seller>()), Times.Never);
        }

        [Fact]
        public async Task Delete_WithoutCommissions_RemovesAndCommits()
        {
            var seller = ExistingSeller();

            await _service.DeleteAsync(seller.Id);

            _sellers.Verify(r => r.Remove(seller), Times.Once);
            _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
