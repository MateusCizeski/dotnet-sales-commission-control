using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IVendedorRepository
    {
        Task Criar(Vendedor vendedor);
        Task<IReadOnlyList<Vendedor>> Listar();
        Task<Vendedor?> ListarPorId(Guid id);
        Task Remover(Vendedor vendedor);
        Task<bool> VerificarCpfExistente(string cpf);
        Task<bool> VerificarEmailExistente(string email);
        Task<(IReadOnlyList<Vendedor>, int)> ListarPaginado(int page, int pageSize);
    }
}
