using Front.Models;
using Front.Services;
using Microsoft.AspNetCore.Mvc;

namespace Front.Pages.Commissions
{
    /// <summary>Admin sees every commission; a Seller sees only their own (the API enforces it).</summary>
    public class IndexModel : ApiPageModel
    {
        private static readonly string[] ExportHeaders = ["Invoice", "Seller", "Base amount", "Rate %", "Commission", "Status", "Calculated", "Paid"];

        public IndexModel(ApiClient api) : base(api) { }

        public List<CommissionItem> Commissions { get; set; } = [];
        public List<SellerOption> Sellers { get; set; } = [];

        [BindProperty(SupportsGet = true)]
        public CommissionStatus? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid? SellerId { get; set; }

        public override Dictionary<string, string> RouteFilters => new()
        {
            ["Status"] = Status?.ToString() ?? string.Empty,
            ["SellerId"] = SellerId?.ToString() ?? string.Empty,
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
            Commissions = page.Content;
            TotalItems = page.Total;
        }

        public async Task<IActionResult> OnGetExportExcelAsync()
        {
            var page = await LoadAsync();
            return File(Exporter.ToExcel("Commissions", ExportHeaders, Rows(page.Content)), Exporter.ExcelContentType, $"Commissions_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        public async Task<IActionResult> OnGetExportPdfAsync()
        {
            var page = await LoadAsync();
            return File(Exporter.ToPdf("Commissions", ExportHeaders, Rows(page.Content)), "application/pdf", $"Commissions_{DateTime.Now:yyyyMMdd}.pdf");
        }

        public Task<IActionResult> OnPostPayAsync(Guid id) =>
            RunAndRedirectAsync(Api.PostAsync($"/api/commissions/{id}/pay"), "Commission marked as paid.", RouteFilters);

        public Task<IActionResult> OnPostCancelAsync(Guid id) =>
            RunAndRedirectAsync(Api.PostAsync($"/api/commissions/{id}/cancel"), "Commission cancelled.", RouteFilters);

        private Task<PageResult<CommissionItem>> LoadAsync()
        {
            NormalizePaging();

            // Filtered by the API (the old page filtered only the current page in memory).
            var filters = new List<object>();

            if (Status.HasValue)
            {
                filters.Add(new { property = "Status", value = Status.Value.ToString() });
            }

            if (SellerId.HasValue)
            {
                filters.Add(new { property = "Invoice.SellerId", value = SellerId.Value });
            }

            return Api.GetPageAsync<CommissionItem>("/api/commissions", CurrentPage, PageSize, [.. filters],
                [new { property = "CalculatedAt", direction = "desc" }]);
        }

        private static IEnumerable<object?[]> Rows(IEnumerable<CommissionItem> commissions) =>
            commissions.Select(c => new object?[]
            {
                c.Invoice?.Number, c.Invoice?.Seller?.FullName, c.BaseAmount, c.Rate, c.Amount, c.Status.ToString(), c.CalculatedAt, c.PaidAt
            });
    }
}
