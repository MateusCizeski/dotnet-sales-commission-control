using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IVendedorRepository
    {
        Task<Vendedor?> GetByIdAsync(Guid id);
        Task<Vendedor?> GetByCpfAsync(string cpf);
        Task<Vendedor?> GetByEmailAsync(string email);
        Task<IReadOnlyList<Vendedor>> GetAllAsync();
        Task AddAsync(Vendedor vendedor);
        Task UpdateAsync(Vendedor vendedor);
        Task RemoveAsync(Vendedor vendedor);
        Task SaveChangesAsync();
    }
}
