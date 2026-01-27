using Application.DTOs.Vendedor;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Applications
{
    public class VendedorApplication : IVendedorApplication
    {
        private IVendedorRepository _vendedorRepository;

        public VendedorApplication(IVendedorRepository vendedorRepository)
        {
            _vendedorRepository = vendedorRepository;
        }

        public async Task<Guid> CriarAsync(CreateVendedorDto dto)
        {
            var vendedor = new Vendedor(dto.Nome, dto.Documento, dto.Email, dto.PercentualComissao);

            await _vendedorRepository.AddAsync(vendedor);
            return vendedor.Id;
        }

        public async Task InativarAsync(Guid id)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(id);

            vendedor.Inativar();
            await _vendedorRepository.UpdateAsync(vendedor);
        }

        public async Task<VendedorDto> ObterPorIdAsync(Guid id)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(id);

            return new VendedorDto()
            {
                Id = vendedor.Id,
                Ativo = vendedor.Ativo,
                Documento = vendedor.Cpf,
                Nome = vendedor.NomeCompleto,
                PercentualComissao = vendedor.PercentualComissao
            };
        }

        public async Task<IReadOnlyList<VendedorDto>> ObterTodosAsync()
        {
            var vendedores = await _vendedorRepository.GetAllAsync();

            return vendedores.Select(v => new VendedorDto
            {
                Id = v.Id,
                Ativo = v.Ativo,
                Documento = v.Cpf,
                Nome = v.NomeCompleto,
                PercentualComissao = v.PercentualComissao
            }).ToList();
        }
    }
}
