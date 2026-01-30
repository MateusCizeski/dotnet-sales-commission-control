using Application.DTOs.Vendedor;
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

        public async Task<Guid> CriarAsync(CreateVendedorDto dto)
        {
            var vendedor = new Vendedor(dto.NomeCompleto, dto.Cpf, dto.Email, dto.PercentualComissao, dto.Telefone);

            await _vendedorRepository.AddAsync(vendedor);
            await _unitOfWork.CommitAsync();

            return vendedor.Id;
        }

        public async Task Atualizar(Guid id, UpdateVendedorDto dto)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(id) ?? throw new ApplicationException("Vendedor não encontrado");

            vendedor.AtualizarDados(dto.Nome, dto.PercentualComissao);
            await _unitOfWork.CommitAsync();
        }

        public async Task InativarAsync(Guid id)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(id) ?? throw new ApplicationException("Vendedor não encontrado");

            if (await _comissaoRepository.ExisteComissaoParaVendedor(id))
            {
                throw new ApplicationException("Não é permitido inativar um vendedor com comissões registradas");
            }

            vendedor.Inativar();
            await _unitOfWork.CommitAsync();
        }

        public async Task AtivarAsync(Guid id)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(id) ?? throw new ApplicationException("Vendedor não encontrado");

            vendedor.Ativar();
            await _unitOfWork.CommitAsync();
        }

        public async Task Remover(Guid id)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(id) ?? throw new ApplicationException("Vendedor não encontrado");

            if (await _comissaoRepository.ExisteComissaoParaVendedor(id))
            {
                throw new ApplicationException("Não é permitido excluir vendedor com comissões registradas");
            }

            await _vendedorRepository.RemoveAsync(vendedor);
            await _unitOfWork.CommitAsync();
        }

        public async Task<VendedorDto> ObterPorIdAsync(Guid id)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(id) ?? throw new ApplicationException("Vendedor não encontrado");

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

        public async Task<IReadOnlyList<VendedorDto>> ObterTodosAsync()
        {
            var vendedores = await _vendedorRepository.GetAllAsync();

            return vendedores.Select(v => new VendedorDto
            {
                Id = v.Id,
                Nome = v.NomeCompleto,
                Documento = v.Cpf,
                PercentualComissao = v.PercentualComissao,
                Ativo = v.Ativo
            }).ToList();
        }
    }
}
