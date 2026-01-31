using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json.Serialization;

namespace Front.Pages.Invoices
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _client;

        public IndexModel(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("Api");
        }

        public List<InvoiceListDto> Invoices { get; set; } = new();
        public List<VendedorDropdownDto> Vendedores { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public Guid? FilterVendedorId { get; set; }

        public async Task OnGetAsync()
        {
            Vendedores = await ObterVendedoresAsync();

            var query = new Dictionary<string, string>();

            if (FilterVendedorId.HasValue)
            {
                query.Add("vendedorId", FilterVendedorId.Value.ToString());
            }

            var url = QueryHelpers.AddQueryString("/api/invoices", query);

            var invoicesApi = await _client.GetFromJsonAsync<List<InvoiceListDto>>(url);

            if (invoicesApi != null)
            {
                Invoices = invoicesApi;
            }
        }

        public async Task<IActionResult> OnPostAprovarAsync(Guid id)
        {
            await _client.PutAsync($"/api/invoices/{id}/aprovar", null);
            return RedirectToPage(new { FilterVendedorId });
        }

        public async Task<IActionResult> OnPostCancelarAsync(Guid id)
        {
            await _client.PutAsync($"/api/invoices/{id}/cancelar", null);
            return RedirectToPage(new { FilterVendedorId });
        }

        private async Task<List<VendedorDropdownDto>> ObterVendedoresAsync()
        {
            var vendedores = await _client.GetFromJsonAsync<List<VendedorDropdownDto>>("/api/vendedores");

            return vendedores ?? new();
        }
    }

    public class VendedorDropdownDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("nome")]
        public string NomeCompleto { get; set; } = string.Empty;

        [JsonPropertyName("documento")]
        public string Cpf { get; set; } = string.Empty;
    }

    public class InvoiceListDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("numeroInvoice")]
        public string NumeroInvoice { get; set; } = string.Empty;

        [JsonPropertyName("cliente")]
        public string Cliente { get; set; } = string.Empty;
        
        [JsonPropertyName("dataEmissao")]
        public DateTime DataEmissao { get; set; }

        [JsonPropertyName("status")]
        public StatusInvoice Status { get; set; }

        [JsonPropertyName("valorTotal")]
        public decimal ValorTotal { get; set; }

        [JsonPropertyName("vendedor")]
        public VendedorDto Vendedor { get; set; } = new();

        public class VendedorDto
        {
            [JsonPropertyName("id")]
            public Guid Id { get; set; }

            [JsonPropertyName("nomeCompleto")]
            public string NomeCompleto { get; set; } = string.Empty;
        }
    }
}
