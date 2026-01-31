using Application.DTOs.Shared;
using Application.DTOs.Vendedor;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Applications
{
    public class VendedorApplication : IVendedorApplication
    {
        private readonly IVendedorRepository _vendedorRepository;
        private readonly IComissaoRepository _comissaoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VendedorApplication(IVendedorRepository vendedorRepository, IComissaoRepository comissaoRepository, IUnitOfWork unitOfWork)
        {
            _vendedorRepository = vendedorRepository;
            _comissaoRepository = comissaoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Criar(CriarVendedorDto dto)
        {
            var vendedor = new Vendedor(dto.NomeCompleto, dto.Cpf, dto.Email, dto.PercentualComissao, dto.Telefone);

            if(await _vendedorRepository.VerificarCpfExistente(dto.Cpf))
            {
                throw new BusinessRuleException("Existe um vendedor com o mesmo CPF.");
            }

            if (await _vendedorRepository.VerificarEmailExistente(dto.Email))
            {
                throw new BusinessRuleException("Existe um vendedor com o mesmo Email.");
            }

            await _vendedorRepository.Criar(vendedor);
            await _unitOfWork.CommitAsync();

            return vendedor.Id;
        }

        public async Task Editar(Guid id, EditarVendedorDto dto)
        {
            var vendedor = await _vendedorRepository.ListarPorId(id) ?? throw new ApplicationException("Vendedor não encontrado");

            vendedor.AtualizarDados(dto.Nome, dto.PercentualComissao);
            await _unitOfWork.CommitAsync();
        }

        public async Task Inativar(Guid id)
        {
            var vendedor = await _vendedorRepository.ListarPorId(id) ?? throw new ApplicationException("Vendedor não encontrado");

            if (await _comissaoRepository.ExisteComissaoParaVendedor(id))
            {
                throw new BusinessRuleException("Não é permitido inativar um vendedor com comissões registradas");
            }

            vendedor.Inativar();
            await _unitOfWork.CommitAsync();
        }

        public async Task Ativar(Guid id)
        {
            var vendedor = await _vendedorRepository.ListarPorId(id) ?? throw new ApplicationException("Vendedor não encontrado");

            vendedor.Ativar();
            await _unitOfWork.CommitAsync();
        }

        public async Task Remover(Guid id)
        {
            var vendedor = await _vendedorRepository.ListarPorId(id) ?? throw new ApplicationException("Vendedor não encontrado");

            if (await _comissaoRepository.ExisteComissaoParaVendedor(id))
            {
                throw new BusinessRuleException("Não é permitido excluir vendedor com comissões registradas");
            }

            await _vendedorRepository.Remover(vendedor);
            await _unitOfWork.CommitAsync();
        }

        public async Task<VendedorDto> ListarPorId(Guid id)
        {
            var vendedor = await _vendedorRepository.ListarPorId(id) ?? throw new ApplicationException("Vendedor não encontrado");

            return new VendedorDto
            {
                Id = vendedor.Id,
                Nome = vendedor.NomeCompleto,
                Documento = vendedor.Cpf,
                Email = vendedor.Email,
                PercentualComissao = vendedor.PercentualComissao,
                Ativo = vendedor.Ativo
            };
        }

        public async Task<IReadOnlyList<VendedorDto>> Listar()
        {
            var vendedores = await _vendedorRepository.Listar();

            return vendedores.Select(v => new VendedorDto
            {
                Id = v.Id,
                Email = v.Email,
                Nome = v.NomeCompleto,
                Documento = v.Cpf,
                PercentualComissao = v.PercentualComissao,
                Ativo = v.Ativo
            }).ToList();
        }

        public async Task<PagedResult<VendedorDto>> ListarPaginado(int page, int pageSize)
        {
            var (items, total) = await _vendedorRepository.ListarPaginado(page, pageSize);

            return new PagedResult<VendedorDto>
            {
                Items = items.Select(v => new VendedorDto
                {
                    Id = v.Id,
                    Email = v.Email,
                    Nome = v.NomeCompleto,
                    Documento = v.Cpf,
                    PercentualComissao = v.PercentualComissao,
                    Ativo = v.Ativo
                }).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalItems = total
            };
        }
    }
}
