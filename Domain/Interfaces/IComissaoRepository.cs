using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IComissaoRepository
    {
        Task Criar(Comissao comissao);
        Task<IReadOnlyList<Comissao>> Listar();
        IQueryable<Comissao> Query();
        Task<Comissao?> ListarPorId(Guid id);
        Task<Comissao?> ListarPorInvoice(Guid invoiceId);
        Task<bool> ExisteComissaoParaVendedor(Guid vendedorId);
        Task<(IReadOnlyList<Comissao>, int)> ListarPaginado(int page, int pageSize);
    }
}
