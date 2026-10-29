using Front.Models;
using Front.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Front.Pages.Sellers
{
    [Authorize(Roles = FrontClaims.AdminRole)]
    public class IndexModel : ApiPageModel
    {
        private static readonly string[] ExportHeaders = ["Name", "CPF", "Email", "Commission %", "Status"];

        public IndexModel(ApiClient api) : base(api) { }

        public List<SellerItem> Sellers { get; set; } = [];

        [BindProperty(SupportsGet = true)]
        public bool? Active { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public async Task OnGetAsync()
        {
            var page = await LoadAsync();
            Sellers = page.Content;
            TotalItems = page.Total;
        }

        public async Task<IActionResult> OnGetExportExcelAsync()
        {
            var page = await LoadAsync();
            return File(Exporter.ToExcel("Sellers", ExportHeaders, Rows(page.Content)), Exporter.ExcelContentType, $"Sellers_{DateTime.Now:yyyyMMdd}.xlsx");
        }

        public async Task<IActionResult> OnGetExportPdfAsync()
        {
            var page = await LoadAsync();
            return File(Exporter.ToPdf("Sellers", ExportHeaders, Rows(page.Content)), "application/pdf", $"Sellers_{DateTime.Now:yyyyMMdd}.pdf");
        }

        public Task<IActionResult> OnPostDeactivateAsync(Guid id) =>
            RunAndRedirectAsync(Api.PostAsync($"/api/sellers/{id}/deactivate"), "Seller deactivated.", RouteFilters);

        public Task<IActionResult> OnPostActivateAsync(Guid id) =>
            RunAndRedirectAsync(Api.PostAsync($"/api/sellers/{id}/activate"), "Seller activated.", RouteFilters);

        public Task<IActionResult> OnPostDeleteAsync(Guid id) =>
            RunAndRedirectAsync(Api.DeleteAsync($"/api/sellers/{id}"), "Seller deleted.", RouteFilters);


        /// <summary>Current filters, used by the export links.</summary>
        public override Dictionary<string, string> RouteFilters => new()
        {
            ["Active"] = Active?.ToString() ?? string.Empty,
            ["Search"] = Search ?? string.Empty,
            ["CurrentPage"] = CurrentPage.ToString(),
            ["PageSize"] = PageSize.ToString()
        };

        private Task<PageResult<SellerItem>> LoadAsync()
        {
            NormalizePaging();

            var filters = new List<object>();

            if (Active.HasValue)
            {
                filters.Add(new { property = "IsActive", value = Active.Value });
            }

            if (!string.IsNullOrWhiteSpace(Search))
            {
                filters.Add(new { property = "FullName", @operator = "contains", value = Search.Trim() });
            }

            return Api.GetPageAsync<SellerItem>("/api/sellers", CurrentPage, PageSize, [.. filters], [new { property = "FullName", direction = "asc" }]);
        }

        private static IEnumerable<object?[]> Rows(IEnumerable<SellerItem> sellers) =>
            sellers.Select(s => new object?[] { s.FullName, s.Cpf, s.Email, s.CommissionRate, s.IsActive ? "Active" : "Inactive" });
    }
}
