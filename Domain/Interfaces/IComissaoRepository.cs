using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IComissaoRepository
    {
        Task<Comissao?> GetByIdAsync(Guid id);
        Task<Comissao?> GetByInvoiceIdAsync(Guid invoiceId);
        Task AddAsync(Comissao comissao);
        Task<bool> ExisteComissaoParaVendedor(Guid vendedorId);
        Task<IReadOnlyList<Comissao>> GetAllAsync();
        IQueryable<Comissao> Query();
    }
}
