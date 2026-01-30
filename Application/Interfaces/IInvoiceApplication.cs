using Application.DTOs.Invoice;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IInvoiceApplication
    {
        Task<Guid> CriarAsync(CreateInvoiceDto dto);
        Task AprovarAsync(Guid id);
        Task CancelarAsync(Guid id);
        Task UpdateAsync(Guid id, UpdateInvoiceDto dto);
        Task<IReadOnlyList<Invoice>> GetAllAsync(Guid? vendedorId);
        Task<InvoiceEditDto> ObterPorIdAsync(Guid id);
    }
}
