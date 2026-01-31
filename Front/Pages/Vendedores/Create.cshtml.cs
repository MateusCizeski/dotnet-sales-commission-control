using Application.DTOs.Vendedor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

namespace Front.Pages.Vendedores
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _client;

        public CreateModel(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("Api");
        }

        [BindProperty]
        public CriarVendedorDto Vendedor { get; set; } = new();

        public async Task<IActionResult> OnPostAsync()
        {
            if (!decimal.TryParse(Vendedor.PercentualComissao.ToString().Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var percentual))
            {
                ModelState.AddModelError(nameof(Vendedor.PercentualComissao), "Percentual inválido");
                return Page();
            }

            Vendedor.PercentualComissao = percentual;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var response = await _client.PostAsJsonAsync("/api/vendedores", Vendedor);

            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Erro ao criar vendedor: {msg}");
                return Page();
            }

            return RedirectToPage("Index");
        }
    }
}
