using Domain.Entities;

namespace Domain.Interfaces
{
    // Write-side repositories: load and add aggregates. Lists and reports are on the read side (Infra/Queries).
    // Changes are saved by ApiBase's IUnitOfWork.CommitAsync, once per use case.

    public interface ISellerRepository
    {
        Task AddAsync(Seller seller, CancellationToken cancellationToken = default);
        Task<Seller?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        void Remove(Seller seller);
        Task<bool> CpfExistsAsync(string cpf, CancellationToken cancellationToken = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    }

    public interface IInvoiceRepository
    {
        Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);

        /// <summary>Loads the invoice with its commission and seller (the whole aggregate).</summary>
        Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }

    public interface ICommissionRepository
    {
        Task<Commission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsForSellerAsync(Guid sellerId, CancellationToken cancellationToken = default);
    }

    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(CancellationToken cancellationToken = default);
        Task AddAsync(User user, CancellationToken cancellationToken = default);
    }
}
