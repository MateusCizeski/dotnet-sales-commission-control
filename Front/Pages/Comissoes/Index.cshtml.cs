using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json.Serialization;

namespace Front.Pages.Comissoes
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _client;

        public IndexModel(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("Api");
        }
        public List<ComissaoListDto> Comissoes { get; set; } = new();
        public string? FiltroStatus { get; set; }

        public async Task OnGetAsync(string? status)
        {
            FiltroStatus = status;

            var lista =
                await _client.GetFromJsonAsync<List<ComissaoListDto>>("/api/comissoes")
                ?? new();

            if (!string.IsNullOrEmpty(status)
                && Enum.TryParse<StatusComissao>(status, out var statusEnum))
            {
                lista = lista
                    .Where(c => c.Status == statusEnum)
                    .ToList();
            }

            Comissoes = lista;
        }

        public async Task<IActionResult> OnPostPagarAsync(Guid id)
        {
            await _client.PutAsync($"/api/comissoes/{id}/pagar", null);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCancelarAsync(Guid id)
        {
            await _client.PutAsync($"/api/comissoes/{id}/cancelar", null);
            return RedirectToPage();
        }
    }

    public class ComissaoListDto
    {
        public Guid Id { get; set; }
        [JsonPropertyName("numeroInvoice")]
        public string InvoiceNumero { get; set; } = "";
        public string VendedorNome { get; set; } = "";
        public decimal ValorBase { get; set; }
        public decimal PercentualAplicado { get; set; }
        public decimal ValorComissao { get; set; }
        public StatusComissao Status { get; set; }
        public DateTime DataCalculo { get; set; }
        public DateTime? DataPagamento { get; set; }
    }
}
