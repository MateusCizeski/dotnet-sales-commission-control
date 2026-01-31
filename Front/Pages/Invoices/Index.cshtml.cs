using Domain.Enums;
using Front.Pages.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using OfficeOpenXml;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Text.Json.Serialization;

namespace Front.Pages.Invoices
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _client;

        public IndexModel(IHttpClientFactory factory)
        {
            _client = factory.CreateClient("Api");
        }

        public List<InvoiceListDto> Invoices { get; set; } = new();
        public List<VendedorDropdownDto> Vendedores { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public Guid? FilterVendedorId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public int TotalPages { get; set; }

        public async Task OnGetAsync()
        {
            if (CurrentPage < 1)
            {
                CurrentPage = 1;
            }

            Vendedores = await ObterVendedoresAsync();

            var query = new Dictionary<string, string>
            {
                { "page", CurrentPage.ToString() },
                { "pageSize", PageSize.ToString() }
            };

            if (FilterVendedorId.HasValue)
            {
                query.Add("vendedorId", FilterVendedorId.Value.ToString());
            }

            var url = QueryHelpers.AddQueryString("/api/invoices", query);

            var invoicesApi = await _client.GetFromJsonAsync<PagedResultDto<InvoiceListDto>>(url);

            if (invoicesApi != null)
            {
                Invoices = invoicesApi.Items;
                CurrentPage = invoicesApi.Page;
                PageSize = invoicesApi.PageSize;
                TotalPages = invoicesApi.TotalPages;
            }
        }

        public async Task<IActionResult> OnPostAprovarAsync(Guid id)
        {
            await _client.PutAsync($"/api/invoices/{id}/aprovar", null);
            return RedirectToPage(new { FilterVendedorId });
        }

        public async Task<IActionResult> OnPostCancelarAsync(Guid id)
        {
            await _client.PutAsync($"/api/invoices/{id}/cancelar", null);
            return RedirectToPage(new { FilterVendedorId });
        }

        private async Task<List<VendedorDropdownDto>> ObterVendedoresAsync()
        {
            var vendedores = await _client.GetFromJsonAsync<List<VendedorDropdownDto>>("/api/vendedores/listar");

            return vendedores ?? new();
        }

        public async Task<IActionResult> OnGetExportExcelAsync()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var query = new Dictionary<string, string>
            {
                { "page", CurrentPage.ToString() },
                { "pageSize", PageSize.ToString() }
            };

            if (FilterVendedorId.HasValue)
            {
                query.Add("vendedorId", FilterVendedorId.Value.ToString());
            }

            var url = QueryHelpers.AddQueryString("/api/invoices", query);

            var invoicesApi = await _client.GetFromJsonAsync<PagedResultDto<InvoiceListDto>>(url);
            var invoices = invoicesApi?.Items ?? new List<InvoiceListDto>();

            using var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Invoices");

            sheet.Cells[1, 1].Value = "Número";
            sheet.Cells[1, 2].Value = "Vendedor";
            sheet.Cells[1, 3].Value = "Cliente";
            sheet.Cells[1, 4].Value = "Valor";
            sheet.Cells[1, 5].Value = "Status";
            sheet.Cells[1, 6].Value = "Emissão";

            for (int i = 0; i < invoices.Count; i++)
            {
                var inv = invoices[i];
                sheet.Cells[i + 2, 1].Value = inv.NumeroInvoice;
                sheet.Cells[i + 2, 2].Value = inv.Vendedor.NomeCompleto;
                sheet.Cells[i + 2, 3].Value = inv.Cliente;
                sheet.Cells[i + 2, 4].Value = inv.ValorTotal;
                sheet.Cells[i + 2, 5].Value = inv.Status.ToString();
                sheet.Cells[i + 2, 6].Value = inv.DataEmissao.ToShortDateString();
            }

            sheet.Cells.AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"Invoices_{DateTime.Now:yyyyMMdd}.xlsx";
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

            if (FilterVendedorId.HasValue)
            {
                query.Add("vendedorId", FilterVendedorId.Value.ToString());
            }

            var url = QueryHelpers.AddQueryString("/api/invoices", query);
            var invoicesApi = await _client.GetFromJsonAsync<PagedResultDto<InvoiceListDto>>(url);
            var invoices = invoicesApi?.Items ?? new List<InvoiceListDto>();

            var fileName = $"Invoices_{DateTime.Now:yyyyMMdd}.pdf";

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Text("Lista de Invoices")
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
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Número");
                            header.Cell().Text("Vendedor");
                            header.Cell().Text("Cliente");
                            header.Cell().Text("Valor");
                            header.Cell().Text("Status");
                            header.Cell().Text("Emissão");
                        });

                        foreach (var inv in invoices)
                        {
                            table.Cell().Text(inv.NumeroInvoice);
                            table.Cell().Text(inv.Vendedor.NomeCompleto);
                            table.Cell().Text(inv.Cliente);
                            table.Cell().Text(inv.ValorTotal.ToString("C"));
                            table.Cell().Text(inv.Status.ToString());
                            table.Cell().Text(inv.DataEmissao.ToShortDateString());
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

    public class VendedorDropdownDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("nome")]
        public string NomeCompleto { get; set; } = string.Empty;

        [JsonPropertyName("documento")]
        public string Cpf { get; set; } = string.Empty;
    }

    public class InvoiceListDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("numeroInvoice")]
        public string NumeroInvoice { get; set; } = string.Empty;

        [JsonPropertyName("cliente")]
        public string Cliente { get; set; } = string.Empty;
        
        [JsonPropertyName("dataEmissao")]
        public DateTime DataEmissao { get; set; }

        [JsonPropertyName("status")]
        public StatusInvoice Status { get; set; }

        [JsonPropertyName("valorTotal")]
        public decimal ValorTotal { get; set; }

        [JsonPropertyName("vendedor")]
        public VendedorDto Vendedor { get; set; } = new();

        public class VendedorDto
        {
            [JsonPropertyName("id")]
            public Guid Id { get; set; }

            [JsonPropertyName("nomeCompleto")]
            public string NomeCompleto { get; set; } = string.Empty;
        }
    }
}
