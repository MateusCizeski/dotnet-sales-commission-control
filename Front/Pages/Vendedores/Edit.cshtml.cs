using Application.DTOs.Vendedor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Front.Pages.Vendedores
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _client;

        public EditModel(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("Api");
        }

        [BindProperty]
        public UpdateVendedorDto Vendedor { get; set; } = new();

        [BindProperty]
        public Guid Id { get; set; }
        public string Cpf { get; set; } = "";
        public string Email { get; set; } = "";

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var vendedor = await _client.GetFromJsonAsync<VendedorDto>($"/api/vendedores/{id}");

            if (vendedor == null)
            {
                return NotFound();
            }

            Id = vendedor.Id;
            Cpf = vendedor.Documento;
            Email = vendedor.Email;

            Vendedor = new UpdateVendedorDto
            {
                Nome = vendedor.Nome,
                PercentualComissao = vendedor.PercentualComissao
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var response = await _client.PutAsJsonAsync($"/api/vendedores/{Id}", Vendedor);

            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Erro ao atualizar vendedor: {msg}");
                return Page();
            }

            return RedirectToPage("Index");
        }
    }
}
