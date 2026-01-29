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

        public InvoiceApplication(IInvoiceRepository invoiceRepository, IVendedorRepository vendedorRepository, IComissaoRepository comissaoRepository, ComissaoService comissaoService)
        {
            _invoiceRepository = invoiceRepository;
            _vendedorRepository = vendedorRepository;
            _comissaoRepository = comissaoRepository;
            _comissaoService = comissaoService;
        }

        public async Task AprovarAsync(Guid id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);

            invoice.Aprovar();
            await _invoiceRepository.UpdateAsync(invoice);
        }

        public async Task CancelarAsync(Guid id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);

            invoice.Cancelar();
            await _invoiceRepository.UpdateAsync(invoice);
        }

        public async Task<Guid> CriarAsync(CreateInvoiceDto dto)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(dto.VendedorId);

            var ultimoNumero = await _invoiceRepository.GetUltimoNumeroAsync();
            var novoNumero = GerarProximoNumero(ultimoNumero);

            var invoice = new Invoice(vendedor, novoNumero, dto.DataEmissao, dto.Cliente, dto.CnpjCpfCliente, dto.ValorTotal, dto.Observacoes);

            var comissao = _comissaoService.Calcular(invoice, vendedor);

            await _invoiceRepository.AddAsync(invoice);
            await _comissaoRepository.AddAsync(comissao);

            return invoice.Id;
        }

        private string GerarProximoNumero(string ultimoNumero)
        {
            if (string.IsNullOrEmpty(ultimoNumero)) return "INV-0001";

            var numero = int.Parse(ultimoNumero.Substring(4));
            numero++;

            return $"INV-{numero:D4}";
        }

        public async Task UpdateAsync(UpdateInvoiceDto dto)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(dto.Id);

            if (invoice == null)
            {
                throw new Exception("Invoice não encontrado");
            }

            if (invoice.Status == StatusInvoice.Aprovada)
            {
                throw new Exception("Não é permitido alterar invoice aprovada.");
            }

            invoice.AlterarVendedor(dto.vendedorId);

            await _invoiceRepository.UpdateAsync(invoice);
            await AtualizarComissaoAsync(invoice);
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


            comissao.AtualizarValores(
                invoice.ValorTotal,
                vendedor.PercentualComissao,
                valorComissao
            );

            await _comissaoRepository.UpdateAsync(comissao);
        }

        public Task<IReadOnlyList<Invoice>> GetAllAsync()
        {
            return _invoiceRepository.GetAllAsync();
        }
    }
}
