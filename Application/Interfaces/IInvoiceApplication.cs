using Application.DTOs.Invoice;

namespace Application.Interfaces
{
    public interface IInvoiceApplication
    {
        Task<Guid> CriarAsync(CreateInvoiceDto dto);
        Task AprovarAsync(Guid id);
        Task CancelarAsync(Guid id);
        Task UpdateAsync(UpdateInvoiceDto dto);
    }
}
