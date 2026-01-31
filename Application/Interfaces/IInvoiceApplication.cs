using Application.DTOs.Invoice;
using Application.DTOs.Shared;

namespace Application.Interfaces
{
    public interface IInvoiceApplication
    {
        Task<Guid> Criar(CriarInvoiceDto dto);
        Task Aprovar(Guid id);
        Task Cancelar(Guid id);
        Task Editar(Guid id, EditarInvoiceDtoEnxuto dto);
        Task<EditarInvoiceDto> ListarPorId(Guid id);
        Task<IReadOnlyList<ListarInvoiceDto>> Listar(Guid vendedorId);
        Task<PagedResult<InvoiceDto>> ListarPaginado(Guid? vendedorId, int page, int pageSize);
    }
}
