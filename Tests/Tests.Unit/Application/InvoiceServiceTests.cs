using ApiBase.Domain.Exceptions;
using ApiBase.Domain.Interfaces;
using Application.Abstractions;
using Application.Requests;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Moq;

namespace Tests.Unit.Application
{
    public class InvoiceServiceTests
    {
        private readonly Mock<IInvoiceRepository> _invoices = new();
        private readonly Mock<ISellerRepository> _sellers = new();
        private readonly Mock<IInvoiceNumberGenerator> _numbers = new();
        private readonly Mock<IUnitOfWork> _unitOfWork = new();
        private readonly InvoiceService _service;

        public InvoiceServiceTests()
        {
            _numbers.Setup(n => n.NextAsync(It.IsAny<CancellationToken>())).ReturnsAsync("INV-0042");
            _service = new InvoiceService(_invoices.Object, _sellers.Object, _numbers.Object, _unitOfWork.Object, new FixedTimeProvider(TestData.Now));
        }

        private static CreateInvoiceRequest Request(Guid sellerId) => new()
        {
            SellerId = sellerId,
            TotalAmount = 1000,
            IssueDate = TestData.Now.Date,
            CustomerName = "Customer Ltd",
            CustomerTaxId = TestData.ValidCnpj
        };

        [Fact]
        public async Task Create_AddsInvoiceWithCommissionAndCommitsOnce()
        {
            var seller = TestData.Seller(rate: 10);
            _sellers.Setup(r => r.GetByIdAsync(seller.Id, It.IsAny<CancellationToken>())).ReturnsAsync(seller);
            Invoice? added = null;
            _invoices.Setup(r => r.AddAsync(It.IsAny<Invoice>(), It.IsAny<CancellationToken>())).Callback<Invoice, CancellationToken>((i, _) => added = i);

            var id = await _service.CreateAsync(Request(seller.Id));

            Assert.NotNull(added);
            Assert.Equal(id, added.Id);
            Assert.Equal("INV-0042", added.Number);
            Assert.Equal(100m, added.Commission.Amount);
            Assert.Equal(TestData.Now, added.Commission.CalculatedAt);
            // One commit: invoice and commission are saved in the same transaction.
            _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_UnknownSeller_ThrowsNotFoundAndDoesNotCommit()
        {
            _sellers.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Seller?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateAsync(Request(Guid.NewGuid())));

            _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Update_RecalculatesCommissionAndCommitsOnce()
        {
            var invoice = TestData.Invoice(TestData.Seller(rate: 10), total: 1000m);
            var newSeller = new Seller("Other", "52998224725", "other@example.com", 5, TestData.Now);
            _invoices.Setup(r => r.GetByIdAsync(invoice.Id, It.IsAny<CancellationToken>())).ReturnsAsync(invoice);
            _sellers.Setup(r => r.GetByIdAsync(newSeller.Id, It.IsAny<CancellationToken>())).ReturnsAsync(newSeller);

            await _service.UpdateAsync(invoice.Id, new UpdateInvoiceRequest { SellerId = newSeller.Id, TotalAmount = 3000 });

            Assert.Equal(150m, invoice.Commission.Amount);
            _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Approve_UnknownInvoice_ThrowsNotFound()
        {
            _invoices.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Invoice?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.ApproveAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task Cancel_CancelsInvoiceAndCommission()
        {
            var invoice = TestData.Invoice();
            _invoices.Setup(r => r.GetByIdAsync(invoice.Id, It.IsAny<CancellationToken>())).ReturnsAsync(invoice);

            await _service.CancelAsync(invoice.Id);

            Assert.Equal(InvoiceStatus.Cancelled, invoice.Status);
            Assert.Equal(CommissionStatus.Cancelled, invoice.Commission.Status);
            _unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
