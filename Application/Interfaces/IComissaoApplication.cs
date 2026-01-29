using Domain.Entities;

namespace Application.Interfaces
{
    public interface IComissaoApplication
    {
        Task MarcarComoPagaAsync(Guid id);
        Task MarcarComoCanceladaAsync(Guid id);
        Task<IReadOnlyList<Comissao>> GetAllAsync();
    }
}
