using ApiBase.Domain.Exceptions;
using ApiBase.Domain.Interfaces;
using Application.Abstractions;
using Application.Requests;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public interface IInvoiceService
    {
        Task<Guid> CreateAsync(CreateInvoiceRequest request, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid id, UpdateInvoiceRequest request, CancellationToken cancellationToken = default);
        Task ApproveAsync(Guid id, CancellationToken cancellationToken = default);
        Task CancelAsync(Guid id, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Invoice commands. The invoice (aggregate root) creates and recalculates its commission,
    /// so each use case adds/changes one aggregate and commits once — invoice and commission
    /// are saved in the same transaction.
    /// </summary>
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ISellerRepository _sellerRepository;
        private readonly IInvoiceNumberGenerator _numberGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public InvoiceService(IInvoiceRepository invoiceRepository, ISellerRepository sellerRepository, IInvoiceNumberGenerator numberGenerator, IUnitOfWork unitOfWork, TimeProvider timeProvider)
        {
            _invoiceRepository = invoiceRepository;
            _sellerRepository = sellerRepository;
            _numberGenerator = numberGenerator;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<Guid> CreateAsync(CreateInvoiceRequest request, CancellationToken cancellationToken = default)
        {
            var seller = await GetSellerAsync(request.SellerId, cancellationToken);
            var number = await _numberGenerator.NextAsync(cancellationToken);

            var invoice = new Invoice(number, seller, request.IssueDate, request.CustomerName, request.CustomerTaxId, request.TotalAmount, request.Notes, Now());

            await _invoiceRepository.AddAsync(invoice, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return invoice.Id;
        }

        public async Task UpdateAsync(Guid id, UpdateInvoiceRequest request, CancellationToken cancellationToken = default)
        {
            var invoice = await GetInvoiceAsync(id, cancellationToken);
            var seller = await GetSellerAsync(request.SellerId, cancellationToken);

            invoice.Update(seller, request.TotalAmount, Now());
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task ApproveAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var invoice = await GetInvoiceAsync(id, cancellationToken);

            invoice.Approve();
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task CancelAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var invoice = await GetInvoiceAsync(id, cancellationToken);

            invoice.Cancel();
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        private DateTime Now() => _timeProvider.GetUtcNow().UtcDateTime;

        private async Task<Invoice> GetInvoiceAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _invoiceRepository.GetByIdAsync(id, cancellationToken) ?? throw NotFoundException.For<Invoice>(id);
        }

        private async Task<Seller> GetSellerAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _sellerRepository.GetByIdAsync(id, cancellationToken) ?? throw NotFoundException.For<Seller>(id);
        }
    }
}
