using Front.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Front.Pages
{
    /// <summary>Base for the pages: API client, current role, paging and the success/error messages.</summary>
    public abstract class ApiPageModel : PageModel
    {
        public static readonly int[] PageSizes = [10, 25, 50, 100];

        protected ApiPageModel(ApiClient api)
        {
            Api = api;
        }

        protected ApiClient Api { get; }

        public bool IsAdmin => User.IsInRole(FrontClaims.AdminRole);

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 10;

        public int TotalItems { get; set; }

        public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalItems / (double)PageSize));

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        /// <summary>Current filters and page as route values (export links, redirects after a command).</summary>
        public virtual Dictionary<string, string> RouteFilters => new()
        {
            ["CurrentPage"] = CurrentPage.ToString(),
            ["PageSize"] = PageSize.ToString()
        };

        /// <summary>
        /// Route values for a row action: the current filters plus the id.
        /// (asp-all-route-data replaces asp-route-id, so the id must be inside the same dictionary.)
        /// </summary>
        public Dictionary<string, string> RouteFor(Guid id)
        {
            var values = RouteFilters;
            values["id"] = id.ToString();
            return values;
        }

        protected void NormalizePaging()
        {
            CurrentPage = Math.Max(1, CurrentPage);
            PageSize = PageSizes.Contains(PageSize) ? PageSize : 10;
        }

        /// <summary>Runs a command and stores the outcome for the next page (Post/Redirect/Get).</summary>
        protected async Task<IActionResult> RunAndRedirectAsync(Task<ApiResult> command, string successMessage, object? routeValues = null)
        {
            var result = await command;

            if (result.Success)
            {
                SuccessMessage = successMessage;
            }
            else
            {
                ErrorMessage = result.Error;
            }

            return RedirectToPage(routeValues);
        }
    }
}
