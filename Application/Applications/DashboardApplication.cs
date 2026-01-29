using Application.DTOs.Dashboard;
using Application.Interfaces;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public async Task<InvoiceSummaryDto> ObterResumoInvoicesAsync(ObterResumoDto dto)
        {
            var invoicesQuery = _invoiceRepository.Query();
            var comissoesQuery = _comissaoRepository.Query();

            if (dto.startDate.HasValue)
            {
                invoicesQuery = invoicesQuery
                    .Where(i => i.DataEmissao >= dto.startDate.Value.Date);
            }

            if (dto.endDate.HasValue)
            {
                invoicesQuery = invoicesQuery
                    .Where(i => i.DataEmissao <= dto.endDate.Value.Date);
            }

            if (dto.vendedorId.HasValue)
            {
                invoicesQuery = invoicesQuery
                    .Where(i => i.VendedorId == dto.vendedorId.Value);
            }

            if (dto.statusInvoice.HasValue)
            {
                invoicesQuery = invoicesQuery
                    .Where(i => i.Status == dto.statusInvoice.Value);
            }

            var invoices = await invoicesQuery.ToListAsync();

            var invoiceIds = invoices.Select(i => i.Id).ToList();

            var comissoes = await comissoesQuery
                .Where(c => invoiceIds.Contains(c.InvoiceId))
                .ToListAsync();

            var totalInvoices = invoices.Count;
            var totalPendente = invoices.Count(i => i.Status == StatusInvoice.Pendente);
            var totalAprovada = invoices.Count(i => i.Status == StatusInvoice.Aprovada);
            var totalCancelada = invoices.Count(i => i.Status == StatusInvoice.Cancelada);

            var valorTotalAprovadas = invoices
                .Where(i => i.Status == StatusInvoice.Aprovada)
                .Sum(i => i.ValorTotal);

            var invoicesUltimos30Dias = invoices
                .Count(i => i.DataEmissao >= DateTime.UtcNow.AddDays(-30));

            var totalComissoesPendentes = comissoes
                .Where(c => c.Status == StatusComissao.Pendente)
                .Sum(c => c.ValorComissao);

            var totalComissoesPagas = comissoes
                .Where(c => c.Status == StatusComissao.Paga)
                .Sum(c => c.ValorComissao);

            var topVendedores = comissoes
                .Where(c => c.Invoice != null)
                .GroupBy(c => new
                {
                    c.Invoice.VendedorId,
                    c.Invoice.Vendedor.NomeCompleto
                })
                .Select(g => new VendedorDashboardDto
                {
                    VendedorId = g.Key.VendedorId,
                    NomeCompleto = g.Key.NomeCompleto,
                    ComissaoTotal = g.Sum(c => c.ValorComissao)
                })
                .OrderByDescending(v => v.ComissaoTotal)
                .Take(5)
                .ToList();

            return new InvoiceSummaryDto
            {
                TotalInvoices = totalInvoices,
                TotalPendente = totalPendente,
                TotalAprovada = totalAprovada,
                TotalCancelada = totalCancelada,
                ValorTotalAprovadas = valorTotalAprovadas,
                InvoicesUltimos30Dias = invoicesUltimos30Dias,
                TotalComissoesPendentes = totalComissoesPendentes,
                TotalComissoesPagas = totalComissoesPagas,
                TopVendedores = topVendedores
            };
        }
    }
}
