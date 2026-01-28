using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Front.Pages.Comissoes
{
    public class IndexModel : PageModel
    {
        public List<Comissao> Comissoes { get; set; } = new();

        public async Task OnGetAsync()
        {
            // GET /api/comissoes
            // Comissoes = await HttpClient.GetFromJsonAsync<List<Comissao>>(...)
        }

        public async Task<IActionResult> OnPostPagarAsync(Guid id)
        {
            // PUT /api/comissoes/{id}/pagar
            // await HttpClient.PutAsync(...)
            return RedirectToPage();
        }
    }
}
