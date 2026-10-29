using Front.Models;
using Front.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Front.Pages.Dashboard
{
    public class IndexModel : ApiPageModel
    {
        public IndexModel(ApiClient api) : base(api) { }

        public DashboardSummary Summary { get; set; } = new();
        public List<SellerOption> Sellers { get; set; } = [];

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid? SellerId { get; set; }

        [BindProperty(SupportsGet = true)]
        public InvoiceStatus? Status { get; set; }

        public async Task OnGetAsync()
        {
            if (IsAdmin)
            {
                Sellers = await Api.GetAsync<List<SellerOption>>("/api/sellers/options");
            }

            var query = new Dictionary<string, string?>
            {
                ["startDate"] = StartDate?.ToString("yyyy-MM-dd"),
                ["endDate"] = EndDate?.ToString("yyyy-MM-dd"),
                ["sellerId"] = SellerId?.ToString(),
                ["status"] = Status?.ToString()
            };

            var url = QueryHelpers.AddQueryString("/api/dashboard/summary", query.Where(q => q.Value != null));
            Summary = await Api.GetAsync<DashboardSummary>(url);
        }
    }
}
