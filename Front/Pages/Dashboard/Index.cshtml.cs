using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace Front.Pages.Dashboard
{
    public class IndexModel : PageModel
    {
        public int TotalInvoices { get; set; }
        public int PendingInvoices { get; set; }
        public decimal TotalAprovadas { get; set; }
        public decimal PendingCommissions { get; set; }
        public List<VendedorDashboardDto> TopVendedores { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterStartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterEndDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid? FilterVendedorId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? FilterStatus { get; set; }

        public List<VendedorDropdownDto> Vendedores { get; set; } = new();

        public async Task OnGetAsync()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:5001");

            //var vendedoresApi = await client.GetFromJsonAsync<List<VendedorDropdownDto>>("/api/vendedores");
            //if (vendedoresApi != null) Vendedores = vendedoresApi;

            var query = new Dictionary<string, string>();
            if (FilterStartDate.HasValue) query.Add("startDate", FilterStartDate.Value.ToString("yyyy-MM-dd"));
            if (FilterEndDate.HasValue) query.Add("endDate", FilterEndDate.Value.ToString("yyyy-MM-dd"));
            if (FilterVendedorId.HasValue) query.Add("vendedorId", FilterVendedorId.Value.ToString());
            if (!string.IsNullOrEmpty(FilterStatus)) query.Add("status", FilterStatus);

            var url = QueryHelpers.AddQueryString("/api/dashboard/invoices/summary", query);

            //var resumo = await client.GetFromJsonAsync<InvoiceSummaryDto>(url);
            var resumo = new InvoiceSummaryDto();

            if (resumo != null)
            {
                TotalInvoices = resumo.TotalInvoices;
                PendingInvoices = resumo.Pending;
                TotalAprovadas = resumo.TotalAprovadas;
                PendingCommissions = resumo.PendingCommissions;
                TopVendedores = resumo.TopVendedores ?? new List<VendedorDashboardDto>();
            }
        }

        public class VendedorDashboardDto
        {
            public string NomeCompleto { get; set; } = "";
            public decimal ComissaoTotal { get; set; }
        }

        public class VendedorDropdownDto
        {
            public Guid Id { get; set; }
            public string NomeCompleto { get; set; } = "";
        }

        public class InvoiceSummaryDto
        {
            public int TotalInvoices { get; set; }
            public int Pending { get; set; }
            public decimal TotalAprovadas { get; set; }
            public decimal PendingCommissions { get; set; }
            public List<VendedorDashboardDto>? TopVendedores { get; set; }
        }
    }
}
