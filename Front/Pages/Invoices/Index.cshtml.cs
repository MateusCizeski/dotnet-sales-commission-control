using Application.DTOs.Invoice;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Front.Pages.Invoices
{
    public class IndexModel : PageModel
    {
        public List<InvoiceListDto> Invoices { get; set; } = new();
        public List<VendedorDropdownDto> Vendedores { get; set; } = new();

        public async Task OnGetAsync()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:5001");

            //var invoicesApi = await client.GetFromJsonAsync<List<InvoiceListDto>>("/api/invoices");
            var invoicesApi = new List<InvoiceListDto>();
            if (invoicesApi != null) Invoices = invoicesApi;

            //var vendedoresApi = await client.GetFromJsonAsync<List<VendedorDropdownDto>>("/api/vendedores");
            var vendedoresApi = new List<VendedorDropdownDto>();
            if (vendedoresApi != null) Vendedores = vendedoresApi;
        }

        public async Task<IActionResult> OnPostCreateAsync([FromForm] CreateInvoiceDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:5001");

            var response = await client.PostAsJsonAsync("/api/invoices", dto);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Erro ao criar invoice");
                return Page();
            }
        }

        public class InvoiceListDto
        {
            public Guid Id { get; set; }
            public string NumeroInvoice { get; set; } = "";
            public string VendedorNome { get; set; } = "";
            public string Cliente { get; set; } = "";
            public decimal ValorTotal { get; set; }
            public StatusInvoice Status { get; set; }
            public DateTime DataEmissao { get; set; }
        }

        public class VendedorDropdownDto
        {
            public Guid Id { get; set; }
            public string NomeCompleto { get; set; } = "";
        }
    }
}
