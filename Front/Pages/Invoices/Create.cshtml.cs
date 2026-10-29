using Front.Models;
using Front.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Front.Pages.Invoices
{
    [Authorize(Roles = FrontClaims.AdminRole)]
    public class CreateModel : ApiPageModel
    {
        public CreateModel(ApiClient api) : base(api) { }

        [BindProperty]
        public InvoiceForm Input { get; set; } = new();

        public List<SelectListItem> Sellers { get; set; } = [];

        public async Task OnGetAsync()
        {
            await LoadSellersAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSellersAsync();
                return Page();
            }

            var result = await Api.PostAsync("/api/invoices", Input);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                await LoadSellersAsync();
                return Page();
            }

            SuccessMessage = "Invoice created with its commission.";
            return RedirectToPage("Index");
        }

        private async Task LoadSellersAsync()
        {
            var sellers = await Api.GetAsync<List<SellerOption>>("/api/sellers/options");
            Sellers = sellers.Select(s => new SelectListItem(s.FullName, s.Id.ToString())).ToList();
        }
    }
}
