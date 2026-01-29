using Application.DTOs.Invoice;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Front.Pages.Invoices
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _client;

        public CreateModel(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("Api");
        }

        [BindProperty]
        public CreateInvoiceDto Invoice { get; set; } = new()
        {
            Status = StatusInvoice.Pendente,
            DataEmissao = DateTime.Today
        };

        public List<SelectListItem> Vendedores { get; set; } = new();

        public async Task OnGetAsync()
        {
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
            if (!ModelState.IsValid)
                return Page();

            var response =
                await _client.PostAsJsonAsync("/api/invoices", Invoice);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Erro ao criar invoice");
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
