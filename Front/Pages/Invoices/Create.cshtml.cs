using Application.DTOs.Invoice;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json.Serialization;

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
            {
                return Page();
            }

            var dto = new Application.DTOs.Invoice.CreateInvoiceDto
            {
                VendedorId = Invoice.VendedorId,
                Cliente = Invoice.Cliente,
                CnpjCpfCliente = Invoice.CnpjCpfCliente,
                ValorTotal = Invoice.ValorTotal,
                DataEmissao = Invoice.DataEmissao,
                Observacoes = Invoice.Observacoes,
                Status = (Domain.Enums.StatusInvoice)Invoice.Status
            };

            var response = await _client.PostAsJsonAsync("/api/invoices", dto);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Erro ao criar invoice");
                return Page();
            }

            return RedirectToPage("Index");
        }

        public class CreateInvoiceViewModel
        {
            public Guid VendedorId { get; set; }
            public decimal ValorTotal { get; set; }
            public DateTime DataEmissao { get; set; }
            public string Cliente { get; set; } = string.Empty;
            public string CnpjCpfCliente { get; set; } = string.Empty;
            public StatusInvoice Status { get; set; }
            public string? Observacoes { get; set; }
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
