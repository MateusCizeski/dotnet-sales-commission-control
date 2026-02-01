using Application.DTOs.Vendedor;
using Front.Pages.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
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

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        public async Task OnGetAsync(bool? ativo)
        {
            FiltroAtivo = ativo;

            if (CurrentPage < 1)
            {
                CurrentPage = 1;
            }

            var query = new Dictionary<string, string>
            {
                { "page", CurrentPage.ToString() },
                { "pageSize", PageSize.ToString() }
            };

            var url = QueryHelpers.AddQueryString("/api/vendedores", query);

            var vendedores = await _client.GetFromJsonAsync<PagedResultDto<VendedorDto>>(url);

            if (ativo.HasValue)
            {
                vendedores.Items = vendedores.Items.Where(v => v.Ativo == ativo.Value).ToList();
            }

            Vendedores = vendedores.Items;
            CurrentPage = vendedores.Page;
            PageSize = vendedores.PageSize;
            TotalPages = vendedores.TotalPages;
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

        public async Task<IActionResult> OnGetExportExcelAsync()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var query = new Dictionary<string, string>
            {
                { "page", CurrentPage.ToString() },
                { "pageSize", PageSize.ToString() }
            };

            if (FiltroAtivo.HasValue)
            {
                query.Add("ativo", FiltroAtivo.Value.ToString());
            }

            var url = QueryHelpers.AddQueryString("/api/vendedores", query);

            var vendedoresApi = await _client.GetFromJsonAsync<PagedResultDto<VendedorDto>>(url);
            var vendedores = vendedoresApi?.Items ?? new List<VendedorDto>();

            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Vendedores");

            sheet.Cells[1, 1].Value = "Nome";
            sheet.Cells[1, 2].Value = "CPF";
            sheet.Cells[1, 3].Value = "Email";
            sheet.Cells[1, 4].Value = "% Comissão";
            sheet.Cells[1, 5].Value = "Status";

            for (int i = 0; i < vendedores.Count; i++)
            {
                var v = vendedores[i];
                sheet.Cells[i + 2, 1].Value = v.Nome;
                sheet.Cells[i + 2, 2].Value = v.Documento;
                sheet.Cells[i + 2, 3].Value = v.Email;
                sheet.Cells[i + 2, 4].Value = v.PercentualComissao;
                sheet.Cells[i + 2, 5].Value = v.Ativo ? "Ativo" : "Inativo";
            }

            sheet.Cells.AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"Vendedores_{DateTime.Now:yyyyMMdd}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public async Task<IActionResult> OnGetExportPdfAsync()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var query = new Dictionary<string, string>
            {
                { "page", CurrentPage.ToString() },
                { "pageSize", PageSize.ToString() }
            };

            if (FiltroAtivo.HasValue)
            {
                query.Add("ativo", FiltroAtivo.Value.ToString());
            }

            var url = QueryHelpers.AddQueryString("/api/vendedores", query);
            var vendedoresApi = await _client.GetFromJsonAsync<PagedResultDto<VendedorDto>>(url);
            var vendedores = vendedoresApi?.Items ?? new List<VendedorDto>();

            var fileName = $"Vendedores_{DateTime.Now:yyyyMMdd}.pdf";

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);

                    page.Header()
                        .Text("Lista de Vendedores")
                        .FontSize(18)
                        .SemiBold()
                        .AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Nome");
                            header.Cell().Text("CPF");
                            header.Cell().Text("Email");
                            header.Cell().Text("% Comissão");
                            header.Cell().Text("Status");
                        });

                        foreach (var v in vendedores)
                        {
                            table.Cell().Text(v.Nome);
                            table.Cell().Text(v.Documento);
                            table.Cell().Text(v.Email);
                            table.Cell().Text(v.PercentualComissao.ToString());
                            table.Cell().Text(v.Ativo ? "Ativo" : "Inativo");
                        }
                    });
                });
            });

            var stream = new MemoryStream();
            pdf.GeneratePdf(stream);
            stream.Position = 0;

            return File(stream, "application/pdf", fileName);
        }
    }
}
