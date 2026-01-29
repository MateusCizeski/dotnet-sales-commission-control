using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IInvoiceRepository
    {
        Task<Invoice?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Invoice>> GetAllAsync();
        Task<IReadOnlyList<Invoice>> GetByVendedorIdAsync(Guid vendedorId);
        Task<IReadOnlyList<Invoice>> GetByPeriodoAsync(DateTime inicio, DateTime fim);
        Task AddAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice);
        Task<string> GetUltimoNumeroAsync();
        IQueryable<Invoice> Query();
    }
}
