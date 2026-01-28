using Application.DTOs.Vendedor;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using System.Text.RegularExpressions;

namespace Application.Applications
{
    public class VendedorApplication : IVendedorApplication
    {
        private IVendedorRepository _vendedorRepository;
        private IComissaoRepository _comissaoRepository;
        private IInvoiceRepository _invoiceRepository;

        public VendedorApplication(IVendedorRepository vendedorRepository, IComissaoRepository comissaoRepository, IInvoiceRepository invoiceRepository)
        {
            _vendedorRepository = vendedorRepository;
            _comissaoRepository = comissaoRepository;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<Guid> CriarAsync(CreateVendedorDto dto)
        {
            var cpfNumeros = Regex.Replace(dto.Cpf, "[^0-9]", "");

            var vendedor = new Vendedor(dto.NomeCompleto, cpfNumeros, dto.Email, dto.PercentualComissao, dto.Telefone);

            await _vendedorRepository.AddAsync(vendedor);
            return vendedor.Id;
        }

        public async Task InativarAsync(Guid id)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(id);

            if (vendedor == null)
            {
                throw new Exception("Vendedor não encontrado");
            }

            var possuiComissao = await _comissaoRepository.ExisteComissaoParaVendedor(id);

            if (possuiComissao)
            {
                throw new Exception("Não é permitido inativar um vendedor que possui comissões registradas");
            }

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

        public async Task Remover(Guid id)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(id);

            if (vendedor == null)
            {
                throw new Exception("Vendedor não encontrado");
            }

            var possuiComissao = await _comissaoRepository.ExisteComissaoParaVendedor(id);

            if (possuiComissao)
            {
                throw new Exception("Não é permitido excluir um vendedor que possui comissões registradas");
            }

            await _vendedorRepository.RemoveAsync(vendedor);
        }
    }
}
