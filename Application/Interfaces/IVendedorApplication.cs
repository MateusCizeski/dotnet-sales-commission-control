using Application.DTOs.Vendedor;

namespace Application.Interfaces
{
    public interface IVendedorApplication
    {
        Task<Guid> CriarAsync(CreateVendedorDto dto);
        Task InativarAsync(Guid id);
        Task<VendedorDto> ObterPorIdAsync(Guid id);
        Task<IReadOnlyList<VendedorDto>> ObterTodosAsync();
    }
}
