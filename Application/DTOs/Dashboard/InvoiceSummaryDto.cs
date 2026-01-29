namespace Application.DTOs.Dashboard
{
    public class InvoiceSummaryDto
    {
        public int TotalInvoices { get; set; }
        public int TotalPendente { get; set; }
        public int TotalAprovada { get; set; }
        public int TotalCancelada { get; set; }
        public decimal ValorTotalAprovadas { get; set; }
        public int InvoicesUltimos30Dias { get; set; }
        public decimal TotalComissoesPendentes { get; set; }
        public decimal TotalComissoesPagas { get; set; }
        public List<VendedorDashboardDto> TopVendedores { get; set; } = new();
    }
}
