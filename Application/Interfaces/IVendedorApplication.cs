using Application.DTOs.Shared;
using Application.DTOs.Vendedor;

namespace Application.Interfaces
{
    public interface IVendedorApplication
    {
        Task<Guid> Criar(CriarVendedorDto dto);
        Task Inativar(Guid id);
        Task Ativar(Guid id);
        Task<VendedorDto> ListarPorId(Guid id);
        Task<IReadOnlyList<VendedorDto>> Listar();
        Task Remover(Guid id);
        Task Editar(Guid id, EditarVendedorDto dto);
        Task<PagedResult<VendedorDto>> ListarPaginado(int page, int pageSize);
    }
}
