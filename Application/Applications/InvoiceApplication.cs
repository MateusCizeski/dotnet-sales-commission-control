using Application.DTOs.Invoice;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Services;

namespace Application.Applications
{
    public class InvoiceApplication : IInvoiceApplication
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IVendedorRepository _vendedorRepository;
        private readonly IComissaoRepository _comissaoRepository;
        private readonly ComissaoService _comissaoService;
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceApplication(
            IInvoiceRepository invoiceRepository,
            IVendedorRepository vendedorRepository,
            IComissaoRepository comissaoRepository,
            ComissaoService comissaoService,
            IUnitOfWork unitOfWork)
        {
            _invoiceRepository = invoiceRepository;
            _vendedorRepository = vendedorRepository;
            _comissaoRepository = comissaoRepository;
            _comissaoService = comissaoService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CriarAsync(CreateInvoiceDto dto)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(dto.VendedorId);

            if (vendedor == null)
            {
                throw new Exception("Vendedor não encontrado");
            }

            var numero = await _invoiceRepository.GetProximoNumeroAsync();

            var invoice = new Invoice(vendedor, numero, dto.DataEmissao, dto.Cliente, dto.CnpjCpfCliente, dto.ValorTotal, dto.Observacoes);

            var comissao = _comissaoService.Calcular(invoice, vendedor);

            await _invoiceRepository.AddAsync(invoice);
            await _comissaoRepository.AddAsync(comissao);

            await _unitOfWork.CommitAsync();

            return invoice.Id;
        }

        public async Task AprovarAsync(Guid id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            
            if (invoice == null)
            {
                throw new Exception("Invoice não encontrada");
            }

            invoice.Aprovar();

            await _invoiceRepository.UpdateAsync(invoice);
            await _unitOfWork.CommitAsync();
        }

        public async Task CancelarAsync(Guid id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            
            if (invoice == null)
            {
                throw new Exception("Invoice não encontrada");
            }

            invoice.Cancelar();

            await _invoiceRepository.UpdateAsync(invoice);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateAsync(Guid id, UpdateInvoiceDto dto)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            
            if (invoice == null)
            {
                throw new Exception("Invoice não encontrada");
            }

            if (invoice.Status == StatusInvoice.Aprovada)
            {
                throw new Exception("Não é permitido alterar invoice aprovada.");
            }

            var vendedor = await _vendedorRepository.GetByIdAsync(dto.VendedorId);

            invoice.AlterarVendedor(vendedor);
            invoice.AlterarValorTotal(dto.ValorTotal);

            await _invoiceRepository.UpdateAsync(invoice);
            await AtualizarComissaoAsync(invoice);

            await _unitOfWork.CommitAsync();
        }

        private async Task AtualizarComissaoAsync(Invoice invoice)
        {
            var comissao = await _comissaoRepository.GetByInvoiceIdAsync(invoice.Id);

            if (comissao == null)
            {
                throw new Exception("Comissão não encontrada para esta invoice.");
            }

            var vendedor = await _vendedorRepository.GetByIdAsync(invoice.VendedorId);

            var valorComissao = Math.Round(invoice.ValorTotal * (vendedor.PercentualComissao / 100), 2);

            comissao.AtualizarValores(invoice.ValorTotal, vendedor.PercentualComissao, valorComissao);

            await _comissaoRepository.UpdateAsync(comissao);
        }

        public Task<IReadOnlyList<Invoice>> GetAllAsync(Guid? vendedorId)
        {
            return _invoiceRepository.GetAllAsync(vendedorId);
        }

        public async Task<InvoiceEditDto> ObterPorIdAsync(Guid id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);

            if (invoice == null)
            {
                throw new Exception("Invoice não encontrada");
            }

            return new InvoiceEditDto
            {
                Id = invoice.Id,
                Numero = invoice.NumeroInvoice,
                Cliente = invoice.Cliente,
                DataEmissao = invoice.DataEmissao,
                Status = invoice.Status.ToString(),
                VendedorId = invoice.VendedorId,
                ValorTotal = invoice.ValorTotal
            };
        }

        public async Task<IReadOnlyList<InvoiceListDto>> ObterTodosDtoAsync(Guid? vendedorId)
        {
            var invoices = await _invoiceRepository.GetAllAsync(vendedorId);
            return invoices.Select(i => new InvoiceListDto
            {
                Id = i.Id,
                Numero = i.NumeroInvoice,
                Cliente = i.Cliente,
                DataEmissao = i.DataEmissao,
                Status = i.Status.ToString(),
                VendedorNome = i.Vendedor.NomeCompleto,
                ValorTotal = i.ValorTotal
            }).ToList();
        }
    }
}
