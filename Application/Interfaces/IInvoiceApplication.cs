using Application.DTOs.Invoice;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IInvoiceApplication
    {
        Task<Guid> CriarAsync(CreateInvoiceDto dto);
        Task AprovarAsync(Guid id);
        Task CancelarAsync(Guid id);
        Task UpdateAsync(UpdateInvoiceDto dto);
        Task<IReadOnlyList<Invoice>> GetAllAsync();
    }
}
