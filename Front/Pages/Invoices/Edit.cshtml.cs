using Application.DTOs.Invoice;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Front.Pages.Invoices
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _client;

        public EditModel(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("Api");
        }
        
        [BindProperty]
        public UpdateInvoiceDto Invoice { get; set; } = new();

        public List<SelectListItem> Vendedores { get; set; } = new();

        public async Task OnGetAsync(Guid id)
        {
            Invoice.Id = id;

            var vendedores =
                await _client.GetFromJsonAsync<List<VendedorDropdownDto>>("/api/vendedores");

            if (vendedores != null)
            {
                Vendedores = vendedores
                    .Select(v => new SelectListItem(v.NomeCompleto, v.Id.ToString()))
                    .ToList();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var response =
                await _client.PutAsJsonAsync("/api/invoices", Invoice);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Erro ao atualizar invoice");
                return Page();
            }

            return RedirectToPage("Index");
        }

        public class VendedorDropdownDto
        {
            public Guid Id { get; set; }
            public string NomeCompleto { get; set; } = "";
        }
    }
}
