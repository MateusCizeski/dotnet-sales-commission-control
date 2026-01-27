using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IComissaoRepository
    {
        Task<Comissao?> GetByIdAsync(Guid id);
        Task<Comissao?> GetByInvoiceIdAsync(Guid invoiceId);
        Task<IReadOnlyList<Comissao>> GetByVendedorIdAsync(Guid vendedorId);
        Task<IReadOnlyList<Comissao>> GetPendentesAsync();
        Task<IReadOnlyList<Comissao>> GetPagasAsync();
        Task AddAsync(Comissao comissao);
        Task UpdateAsync(Comissao comissao);
    }
}
