using Application.DTOs.Comissao;
using Application.DTOs.Shared;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IComissaoApplication
    {
        Task MarcarComoPaga(Guid id);
        Task MarcarComoCancelada(Guid id);
        Task<PagedResult<ComissaoListDto>> ListarPaginado(int page, int pageSize);
    }
}
