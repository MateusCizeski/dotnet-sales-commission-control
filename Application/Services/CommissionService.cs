using ApiBase.Domain.Exceptions;
using ApiBase.Domain.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public interface ICommissionService
    {
        Task PayAsync(Guid id, CancellationToken cancellationToken = default);
        Task CancelAsync(Guid id, CancellationToken cancellationToken = default);
    }

    public class CommissionService : ICommissionService
    {
        private readonly ICommissionRepository _commissionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public CommissionService(ICommissionRepository commissionRepository, IUnitOfWork unitOfWork, TimeProvider timeProvider)
        {
            _commissionRepository = commissionRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task PayAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var commission = await GetAsync(id, cancellationToken);

            commission.Pay(_timeProvider.GetUtcNow().UtcDateTime);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task CancelAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var commission = await GetAsync(id, cancellationToken);

            commission.Cancel();
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        private async Task<Commission> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _commissionRepository.GetByIdAsync(id, cancellationToken) ?? throw NotFoundException.For<Commission>(id);
        }
    }
}
