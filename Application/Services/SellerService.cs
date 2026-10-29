using ApiBase.Domain.Exceptions;
using ApiBase.Domain.Interfaces;
using Application.Exceptions;
using Application.Requests;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public interface ISellerService
    {
        Task<Guid> CreateAsync(CreateSellerRequest request, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid id, UpdateSellerRequest request, CancellationToken cancellationToken = default);
        Task ActivateAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }

    /// <summary>Seller commands. Each method is one use case and commits once.</summary>
    public class SellerService : ISellerService
    {
        private readonly ISellerRepository _sellerRepository;
        private readonly ICommissionRepository _commissionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public SellerService(ISellerRepository sellerRepository, ICommissionRepository commissionRepository, IUnitOfWork unitOfWork, TimeProvider timeProvider)
        {
            _sellerRepository = sellerRepository;
            _commissionRepository = commissionRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<Guid> CreateAsync(CreateSellerRequest request, CancellationToken cancellationToken = default)
        {
            var seller = new Seller(request.FullName, request.Cpf, request.Email, request.CommissionRate, _timeProvider.GetUtcNow().UtcDateTime, request.Phone);

            if (await _sellerRepository.CpfExistsAsync(seller.Cpf, cancellationToken))
            {
                throw new BusinessRuleException("A seller with this CPF already exists.");
            }

            if (await _sellerRepository.EmailExistsAsync(seller.Email, cancellationToken))
            {
                throw new BusinessRuleException("A seller with this email already exists.");
            }

            await _sellerRepository.AddAsync(seller, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return seller.Id;
        }

        public async Task UpdateAsync(Guid id, UpdateSellerRequest request, CancellationToken cancellationToken = default)
        {
            var seller = await GetAsync(id, cancellationToken);

            seller.UpdateDetails(request.FullName, request.CommissionRate);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task ActivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var seller = await GetAsync(id, cancellationToken);

            seller.Activate();
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var seller = await GetAsync(id, cancellationToken);

            if (await _commissionRepository.ExistsForSellerAsync(id, cancellationToken))
            {
                throw new BusinessRuleException("A seller with registered commissions cannot be deactivated.");
            }

            seller.Deactivate();
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var seller = await GetAsync(id, cancellationToken);

            if (await _commissionRepository.ExistsForSellerAsync(id, cancellationToken))
            {
                throw new BusinessRuleException("A seller with registered commissions cannot be deleted.");
            }

            _sellerRepository.Remove(seller);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        private async Task<Seller> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _sellerRepository.GetByIdAsync(id, cancellationToken) ?? throw NotFoundException.For<Seller>(id);
        }
    }
}
