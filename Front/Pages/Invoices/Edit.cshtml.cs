using Application.DTOs.Invoice;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json.Serialization;

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
        public EditarInvoiceDtoEnxuto Invoice { get; set; } = new();

        public List<SelectListItem> Vendedores { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var invoice = await _client.GetFromJsonAsync<EditarInvoiceDto>($"/api/invoices/{id}");

            if (invoice == null)
            {
                return RedirectToPage("Index");
            }

            Invoice.Id = invoice.Id;
            Invoice.VendedorId = invoice.VendedorId;
            Invoice.ValorTotal = invoice.ValorTotal;

            await CarregarVendedoresAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await CarregarVendedoresAsync();
                return Page();
            }

            var response = await _client.PutAsJsonAsync($"/api/invoices/{Invoice.Id}", Invoice);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Erro ao atualizar invoice");
                await CarregarVendedoresAsync();
                return Page();
            }

            return RedirectToPage("Index");
        }

        private async Task CarregarVendedoresAsync()
        {
            var vendedores = await _client.GetFromJsonAsync<List<VendedorDropdownDto>>("/api/vendedores/listar");

            Vendedores = vendedores?
                .Select(v => new SelectListItem
                {
                    Text = v.NomeCompleto,
                    Value = v.Id.ToString(),
                    Selected = v.Id == Invoice.VendedorId
                })
                .ToList() ?? new();
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
    }
}
