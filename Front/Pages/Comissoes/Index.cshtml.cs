using Domain.Enums;
using Front.Pages.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using OfficeOpenXml;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text.Json.Serialization;
using QuestPDF.Fluent;

namespace Front.Pages.Comissoes
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _client;

        public IndexModel(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("Api");
        }
        public List<ComissaoListDto> Comissoes { get; set; } = new();
        public string? FiltroStatus { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid? FilterVendedorId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        public async Task OnGetAsync(string? status)
        {
            FiltroStatus = status;

            if (CurrentPage < 1)
            {
                CurrentPage = 1;
            }

            var query = new Dictionary<string, string>
            {
                { "page", CurrentPage.ToString() },
                { "pageSize", PageSize.ToString() }
            };

            var url = QueryHelpers.AddQueryString("/api/comissoes", query);

            var comissoes = await _client.GetFromJsonAsync<PagedResultDto<ComissaoListDto>>(url);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<StatusComissao>(status, out var statusEnum))
            {
                comissoes.Items = comissoes.Items.Where(c => c.Status == statusEnum).ToList();
            }

            Comissoes = comissoes.Items;
            CurrentPage = comissoes.Page;
            PageSize = comissoes.PageSize;
            TotalPages = comissoes.TotalPages;
        }

        public async Task<IActionResult> OnPostPagarAsync(Guid id)
        {
            await _client.PutAsync($"/api/comissoes/{id}/pagar", null);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCancelarAsync(Guid id)
        {
            await _client.PutAsync($"/api/comissoes/{id}/cancelar", null);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetExportExcelAsync(string? status)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


            var query = new Dictionary<string, string>
            {
                { "page", CurrentPage.ToString() },
                { "pageSize", PageSize.ToString() }
            };

            var url = QueryHelpers.AddQueryString("/api/comissoes", query);
            var comissoesResult = await _client.GetFromJsonAsync<PagedResultDto<ComissaoListDto>>(url);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<StatusComissao>(status, out var statusEnum))
            {
                comissoesResult.Items = comissoesResult.Items.Where(c => c.Status == statusEnum).ToList();
            }

            var comissoes = comissoesResult.Items;

            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Comissoes");

            sheet.Cells[1, 1].Value = "Invoice";
            sheet.Cells[1, 2].Value = "Vendedor";
            sheet.Cells[1, 3].Value = "Valor Base";
            sheet.Cells[1, 4].Value = "%";
            sheet.Cells[1, 5].Value = "Valor Comissão";
            sheet.Cells[1, 6].Value = "Status";
            sheet.Cells[1, 7].Value = "Data Cálculo";
            sheet.Cells[1, 8].Value = "Data Pagamento";

            for (int i = 0; i < comissoes.Count; i++)
            {
                var c = comissoes[i];
                sheet.Cells[i + 2, 1].Value = c.InvoiceNumero;
                sheet.Cells[i + 2, 2].Value = c.VendedorNome;
                sheet.Cells[i + 2, 3].Value = c.ValorBase;
                sheet.Cells[i + 2, 4].Value = c.PercentualAplicado;
                sheet.Cells[i + 2, 5].Value = c.ValorComissao;
                sheet.Cells[i + 2, 6].Value = c.Status.ToString();
                sheet.Cells[i + 2, 7].Value = c.DataCalculo.ToShortDateString();
                sheet.Cells[i + 2, 8].Value = c.DataPagamento?.ToShortDateString() ?? "-";
            }

            sheet.Cells.AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"Comissoes_{DateTime.Now:yyyyMMdd}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public async Task<IActionResult> OnGetExportPdfAsync(string? status)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var query = new Dictionary<string, string>
            {
                { "page", CurrentPage.ToString() },
                { "pa" +
                "geSize", PageSize.ToString() }
            };
            var url = QueryHelpers.AddQueryString("/api/comissoes", query);
            var comissoesResult = await _client.GetFromJsonAsync<PagedResultDto<ComissaoListDto>>(url);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<StatusComissao>(status, out var statusEnum))
            {
                comissoesResult.Items = comissoesResult.Items.Where(c => c.Status == statusEnum).ToList();
            }

            var comissoes = comissoesResult.Items;

            var fileName = $"Comissoes_{DateTime.Now:yyyyMMdd}.pdf";

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Text("Lista de Comissões")
                        .SemiBold().FontSize(16).AlignCenter();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Invoice");
                            header.Cell().Text("Vendedor");
                            header.Cell().Text("Valor Base");
                            header.Cell().Text("%");
                            header.Cell().Text("Comissão");
                            header.Cell().Text("Status");
                            header.Cell().Text("Data Cálc.");
                            header.Cell().Text("Data Pag.");
                        });

                        foreach (var c in comissoes)
                        {
                            table.Cell().Text(c.InvoiceNumero);
                            table.Cell().Text(c.VendedorNome);
                            table.Cell().Text(c.ValorBase.ToString("C"));
                            table.Cell().Text(c.PercentualAplicado.ToString());
                            table.Cell().Text(c.ValorComissao.ToString("C"));
                            table.Cell().Text(c.Status.ToString());
                            table.Cell().Text(c.DataCalculo.ToShortDateString());
                            table.Cell().Text(c.DataPagamento?.ToShortDateString() ?? "-");
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

    public class ComissaoListDto
    {
        public Guid Id { get; set; }
        [JsonPropertyName("numeroInvoice")]
        public string InvoiceNumero { get; set; } = "";
        public string VendedorNome { get; set; } = "";
        public decimal ValorBase { get; set; }
        public decimal PercentualAplicado { get; set; }
        public decimal ValorComissao { get; set; }
        public StatusComissao Status { get; set; }
        public DateTime DataCalculo { get; set; }
        public DateTime? DataPagamento { get; set; }
    }
}
