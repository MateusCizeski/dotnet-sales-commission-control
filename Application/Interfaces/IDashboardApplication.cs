using Application.DTOs.Dashboard;

namespace Application.Interfaces
{
    public interface IDashboardApplication
    {
        Task<InvoiceSummaryDto> ObterResumoInvoicesAsync();
        Task<ComissaoVendedorDto> ObterComissoesPorVendedorAsync(Guid vendedorId);
    }
}
