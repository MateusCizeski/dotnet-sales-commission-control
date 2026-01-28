using Application.DTOs.Vendedor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Front.Pages.Vendedores
{
    public class IndexModel : PageModel
    {
        public List<VendedorListDto> Vendedores { get; set; } = new();

        public async Task OnGetAsync()
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:5001");

            //var list = await client.GetFromJsonAsync<List<VendedorListDto>>("/api/vendedores");
            var list = new List<VendedorListDto>();
            if (list != null) Vendedores = list;
        }

        public async Task<IActionResult> OnPostCreateAsync([FromForm] CreateVendedorDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:5001");

            var response = await client.PostAsJsonAsync("/api/vendedores", dto);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Erro ao criar vendedor");
                return Page();
            }
        }

        public class VendedorListDto
        {
            public Guid Id { get; set; }
            public string NomeCompleto { get; set; } = "";
            public string Cpf { get; set; } = "";
            public string Email { get; set; } = "";
            public decimal PercentualComissao { get; set; }
            public bool Ativo { get; set; }
        }
    }
}
