using Application.DTOs.Dashboard;
using Application.Interfaces;
using Domain.Enums;
using Domain.Interfaces;
using Infra.Data.Repositories;

namespace Application.Applications
{
    public class DashboardApplication : IDashboardApplication
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IComissaoRepository _comissaoRepository;

        public DashboardApplication(IInvoiceRepository invoiceRepository, IComissaoRepository comissaoRepository)
        {
            _invoiceRepository = invoiceRepository;
            _comissaoRepository = comissaoRepository;
        }

        public async Task<ComissaoVendedorDto> ObterComissoesPorVendedorAsync(Guid vendedorId)
        {
            var comissoes = await _comissaoRepository.GetByVendedorIdAsync(vendedorId);
            if (!comissoes.Any()) return new ComissaoVendedorDto { VendedorId = vendedorId };

            var vendedor = comissoes.First().Invoice.Vendedor;

            var dto = new ComissaoVendedorDto
            {
                VendedorId = vendedorId,
                NomeVendedor = vendedor.NomeCompleto,
                TotalComissoes = comissoes.Sum(c => c.ValorComissao),
                TotalPendentes = comissoes.Where(c => c.Status == StatusComissao.Pendente).Sum(c => c.ValorComissao),
                TotalPagas = comissoes.Where(c => c.Status == StatusComissao.Paga).Sum(c => c.ValorComissao),
                Comissoes = comissoes.Select(c => new ComissaoDetalheDto
                {
                    Id = c.Id,
                    InvoiceId = c.InvoiceId,
                    ValorComissao = c.ValorComissao,
                    DataCriacao = c.DataCalculo,
                    Paga = c.Status == StatusComissao.Paga
                }).ToList()
            };

            return dto;
        }

        public async Task<InvoiceSummaryDto> ObterResumoInvoicesAsync()
        {
            var invoices = await _invoiceRepository.GetAllAsync();

            var resumo = new InvoiceSummaryDto
            {
                TotalInvoices = invoices.Count,
                TotalPendente = invoices.Count(i => i.Status == Domain.Enums.StatusInvoice.Pendente),
                TotalAprovada = invoices.Count(i => i.Status == Domain.Enums.StatusInvoice.Aprovada),
                TotalCancelada = invoices.Count(i => i.Status == Domain.Enums.StatusInvoice.Cancelada),
                ValorTotalAprovadas = invoices
                    .Where(i => i.Status == Domain.Enums.StatusInvoice.Aprovada)
                    .Sum(i => i.ValorTotal),
                InvoicesUltimos30Dias = invoices
                    .Count(i => i.DataEmissao >= DateTime.UtcNow.AddDays(-30))
            };

            return resumo;
        }
    }
}
