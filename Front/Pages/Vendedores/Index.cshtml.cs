using Application.DTOs.Vendedor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace Front.Pages.Vendedores
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _client;

        public IndexModel(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("Api");
        }

        public List<VendedorDto> Vendedores { get; set; } = new();
        public bool? FiltroAtivo { get; set; }

        public async Task OnGetAsync(bool? ativo)
        {
            FiltroAtivo = ativo;

            var lista = await _client.GetFromJsonAsync<List<VendedorDto>>("/api/vendedores") ?? [];

            if (ativo.HasValue)
                lista = lista.Where(v => v.Ativo == ativo.Value).ToList();

            Vendedores = lista;
        }

        public async Task<IActionResult> OnPostInativarAsync(Guid id)
        {
            var response = await _client.PutAsync($"/api/vendedores/{id}/inativar", new StringContent("{}", Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                TempData["MensagemErro"] = $"Erro ao inativar: {msg}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAtivarAsync(Guid id)
        {
            var response = await _client.PutAsync($"/api/vendedores/{id}/ativar", new StringContent("{}", Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                TempData["MensagemErro"] = $"Erro ao ativar: {msg}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoverAsync(Guid id)
        {
            var response = await _client.PutAsync($"/api/vendedores/{id}/remover", new StringContent("{}", Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                TempData["MensagemErro"] = $"Erro ao remover: {msg}";
            }

            return RedirectToPage();
        }
    }
}
