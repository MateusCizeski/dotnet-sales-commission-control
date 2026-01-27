using Application.DTOs.Invoice;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Applications
{
    public class InvoiceApplication : IInvoiceApplication
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IVendedorRepository _vendedorRepository;
        private readonly IComissaoRepository _comissaoRepository;

        public InvoiceApplication(IInvoiceRepository invoiceRepository, IVendedorRepository vendedorRepository, IComissaoRepository comissaoRepository)
        {
            _invoiceRepository = invoiceRepository;
            _vendedorRepository = vendedorRepository;
            _comissaoRepository = comissaoRepository;
        }

        public async Task AprovarAsync(Guid id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);

            invoice.Aprovar();
            await _invoiceRepository.UpdateAsync(invoice);
        }

        public async Task<Guid> CriarAsync(CreateInvoiceDto dto)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(dto.VendedorId);
            var invoice = new Invoice(vendedor, dto.NumeroInvoice, dto.DataEmissao, dto.Cliente, dto.CnpjCpfCliente, dto.ValorTotal, dto.Observacoes);

            //var comissao = _comissaoService.Calcular(invoice, vendedor);

            await _invoiceRepository.AddAsync(invoice);
            //await _comissaoRepository.AddAsync(comissao);

            return invoice.Id;
        }
    }
}
