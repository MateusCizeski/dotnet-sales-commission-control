using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IInvoiceRepository
    {
        Task Criar(Invoice invoice);
        Task Editar(Invoice invoice);
        Task<Invoice?> ListarPorId(Guid id);
        Task<IReadOnlyList<Invoice>> Listar(Guid? vendedorId = null);
        IQueryable<Invoice> Query();
        Task<IReadOnlyList<Invoice>> ListarPorPeriodo(DateTime inicio, DateTime fim);
        Task<IReadOnlyList<Invoice>> ListarPorVendedor(Guid vendedorId);
        Task<string?> BuscarUltimoNumeroInvoice();
        Task<string> BuscarProximoNumeroInvoice();
        Task<(IReadOnlyList<Invoice>, int)> ListarPaginado(Guid? vendedorId, int page, int pageSize);
    }
}
