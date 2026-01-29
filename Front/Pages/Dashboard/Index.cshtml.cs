using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json.Serialization;

namespace Front.Pages.Dashboard
{
    public class IndexModel : PageModel
    {
        public int TotalInvoices { get; set; }
        public int PendingInvoices { get; set; }
        public decimal TotalAprovadas { get; set; }
        public decimal PendingCommissions { get; set; }

        public List<VendedorDashboardDto> TopVendedores { get; set; } = new();
        public List<VendedorDropdownDto> Vendedores { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterStartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterEndDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid? FilterVendedorId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FilterStatus { get; set; }

        public async Task OnGetAsync()
        {
            using var client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:5001")
            };

            Vendedores = await ObterVendedoresAsync(client);

            var query = new Dictionary<string, string>();

            if (FilterStartDate.HasValue)
            {
                query.Add("startDate", FilterStartDate.Value.ToString("yyyy-MM-dd"));
            }

            if (FilterEndDate.HasValue)
            {
                query.Add("endDate", FilterEndDate.Value.ToString("yyyy-MM-dd"));
            }

            if (FilterVendedorId.HasValue)
            {
                query.Add("vendedorId", FilterVendedorId.Value.ToString());
            }

            if (!string.IsNullOrWhiteSpace(FilterStatus))
            {
                query.Add("statusInvoice", FilterStatus);
            }

            var url = QueryHelpers.AddQueryString("/api/dashboard/invoices/summary", query);

            var resumo = await client.GetFromJsonAsync<InvoiceSummaryDto>(url);

            if (resumo == null)
            {
                return;
            }

            TotalInvoices = resumo.TotalInvoices;
            PendingInvoices = resumo.TotalPendente;
            TotalAprovadas = resumo.ValorTotalAprovadas;
            PendingCommissions = resumo.TotalComissoesPendentes;
            TopVendedores = resumo.TopVendedores ?? new();
        }

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

        private async Task<List<VendedorDropdownDto>> ObterVendedoresAsync(HttpClient client)
        {
            var vendedores = await client.GetFromJsonAsync<List<VendedorDropdownDto>>("/api/vendedores");

            return vendedores ?? new List<VendedorDropdownDto>();
        }

        public class VendedorDashboardDto
        {
            public string NomeCompleto { get; set; } = string.Empty;
            public decimal ComissaoTotal { get; set; }
        }

        public class VendedorDropdownDto
        {
            [JsonPropertyName("id")]
            public Guid Id { get; set; }
            [JsonPropertyName("documento")]
            public string Cpf { get; set; }
            [JsonPropertyName("nome")]
            public string NomeCompleto { get; set; } = string.Empty;
        }
    }
}
