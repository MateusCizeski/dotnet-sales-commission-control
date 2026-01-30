using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IVendedorRepository
    {
        Task AddAsync(Vendedor vendedor);
        Task<IReadOnlyList<Vendedor>> GetAllAsync();
        Task<Vendedor?> GetByIdAsync(Guid id);
        Task RemoveAsync(Vendedor vendedor);
    }
}
