using Application.DTOs.Comissao;
using Application.DTOs.Shared;
using Application.Interfaces;
using Domain.Interfaces;

namespace Application.Applications
{
    public class ComissaoApplication : IComissaoApplication
    {
        private readonly IComissaoRepository _comissaoRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ComissaoApplication(IComissaoRepository comissaoRepository, IUnitOfWork unitOfWork)
        {
            _comissaoRepository = comissaoRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task MarcarComoPaga(Guid id)
        {
            var comissao = await _comissaoRepository.ListarPorId(id);

            comissao.Pagar();
            await _unitOfWork.CommitAsync();
        }

        public async Task MarcarComoCancelada(Guid id)
        {
            var comissao = await _comissaoRepository.ListarPorId(id);

            comissao.Cancelar();
            await _unitOfWork.CommitAsync();
        }

        public async Task<PagedResult<ComissaoListDto>> ListarPaginado(int page, int pageSize)
        {
            var (items, total) = await _comissaoRepository.ListarPaginado(page, pageSize);

            return new PagedResult<ComissaoListDto>
            {
                Items = items.Select(c => new ComissaoListDto
                {
                    Id = c.Id,
                    NumeroInvoice = c.Invoice.NumeroInvoice,
                    VendedorNome = c.Invoice.Vendedor.NomeCompleto,
                    ValorBase = c.ValorBase,
                    PercentualAplicado = c.PercentualAplicado,
                    ValorComissao = c.ValorComissao,
                    Status = c.Status,
                    DataCalculo = c.DataCalculo,
                    DataPagamento = c.DataPagamento
                }).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalItems = total
            };
        }
    }
}
