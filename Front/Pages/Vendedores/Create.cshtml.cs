using Application.DTOs.Vendedor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
