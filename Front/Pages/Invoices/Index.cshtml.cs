using Front.Models;
using Front.Services;
using Microsoft.AspNetCore.Mvc;

namespace Front.Pages.Invoices
{
    /// <summary>Admin sees every invoice; a Seller sees only their own (the API enforces it).</summary>
    public class IndexModel : ApiPageModel
    {
        private static readonly string[] ExportHeaders = ["Number", "Seller", "Customer", "Amount", "Status", "Issue date"];

        public IndexModel(ApiClient api) : base(api) { }

        public List<InvoiceItem> Invoices { get; set; } = [];
        public List<SellerOption> Sellers { get; set; } = [];

        [BindProperty(SupportsGet = true)]
        public Guid? SellerId { get; set; }

        [BindProperty(SupportsGet = true)]
        public InvoiceStatus? Status { get; set; }

        public override Dictionary<string, string> RouteFilters => new()
        {
            ["SellerId"] = SellerId?.ToString() ?? string.Empty,
            ["Status"] = Status?.ToString() ?? string.Empty,
            ["CurrentPage"] = CurrentPage.ToString(),
            ["PageSize"] = PageSize.ToString()
        };

        public async Task OnGetAsync()
        {
            if (IsAdmin)
            {
                Sellers = await Api.GetAsync<List<SellerOption>>("/api/sellers/options");
            }

            var page = await LoadAsync();
            Invoices = page.Content;
            TotalItems = page.Total;
        }

        public async Task<IActionResult> OnGetExportExcelAsync()
        {
            var page = await LoadAsync();
            return File(Exporter.ToExcel("Invoices", ExportHeaders, Rows(page.Content)), Exporter.ExcelContentType, $"Invoices_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        public async Task<IActionResult> OnGetExportPdfAsync()
        {
            var page = await LoadAsync();
            return File(Exporter.ToPdf("Invoices", ExportHeaders, Rows(page.Content)), "application/pdf", $"Invoices_{DateTime.Now:yyyyMMdd}.pdf");
        }

        public Task<IActionResult> OnPostApproveAsync(Guid id) =>
            RunAndRedirectAsync(Api.PostAsync($"/api/invoices/{id}/approve"), "Invoice approved.", RouteFilters);

        public Task<IActionResult> OnPostCancelAsync(Guid id) =>
            RunAndRedirectAsync(Api.PostAsync($"/api/invoices/{id}/cancel"), "Invoice cancelled.", RouteFilters);

        private Task<PageResult<InvoiceItem>> LoadAsync()
        {
            NormalizePaging();

            var filters = new List<object>();

            if (SellerId.HasValue)
            {
                filters.Add(new { property = "SellerId", value = SellerId.Value });
            }

            if (Status.HasValue)
            {
                filters.Add(new { property = "Status", value = Status.Value.ToString() });
            }

            return Api.GetPageAsync<InvoiceItem>("/api/invoices", CurrentPage, PageSize, [.. filters],
                [new { property = "IssueDate", direction = "desc" }, new { property = "Number", direction = "desc" }]);
        }

        private static IEnumerable<object?[]> Rows(IEnumerable<InvoiceItem> invoices) =>
            invoices.Select(i => new object?[] { i.Number, i.Seller?.FullName, i.CustomerName, i.TotalAmount, i.Status.ToString(), i.IssueDate });
    }
}
