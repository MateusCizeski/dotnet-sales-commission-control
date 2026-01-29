using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Front.Pages.Invoices
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _client;

        public IndexModel(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("Api");
        }

        public List<Invoice> Invoices { get; set; } = new();

        public async Task OnGetAsync()
        {
            var invoicesApi =
                await _client.GetFromJsonAsync<List<Invoice>>("/api/invoices");

            if (invoicesApi != null)
                Invoices = invoicesApi;
        }

        public async Task<IActionResult> OnPostAprovarAsync(Guid id)
        {
            await _client.PutAsync($"/api/invoices/{id}/aprovar", null);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCancelarAsync(Guid id)
        {
            await _client.PutAsync($"/api/invoices/{id}/cancelar", null);
            return RedirectToPage();
        }
    }
}
