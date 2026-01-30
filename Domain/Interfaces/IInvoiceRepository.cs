using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IInvoiceRepository
    {
        Task AddAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice);
        Task<Invoice?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Invoice>> GetAllAsync(Guid? vendedorId = null);
        IQueryable<Invoice> Query();
        Task<IReadOnlyList<Invoice>> GetByPeriodoAsync(DateTime inicio, DateTime fim);
        Task<IReadOnlyList<Invoice>> GetByVendedorIdAsync(Guid vendedorId);
        Task<string?> GetUltimoNumeroAsync();
        Task<string> GetProximoNumeroAsync();
    }
}
