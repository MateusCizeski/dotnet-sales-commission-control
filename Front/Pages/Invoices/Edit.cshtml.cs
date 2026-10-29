using Front.Models;
using Front.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Front.Pages.Invoices
{
    [Authorize(Roles = FrontClaims.AdminRole)]
    public class EditModel : ApiPageModel
    {
        public EditModel(ApiClient api) : base(api) { }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public InvoiceEditForm Input { get; set; } = new();

        public string Number { get; set; } = string.Empty;
        public List<SelectListItem> Sellers { get; set; } = [];

        public async Task<IActionResult> OnGetAsync()
        {
            var invoice = await Api.FindAsync<InvoiceItem>($"/api/invoices/{Id}");

            if (invoice == null)
            {
                return NotFound();
            }

            Number = invoice.Number;
            Input = new InvoiceEditForm { SellerId = invoice.SellerId, TotalAmount = invoice.TotalAmount };
            await LoadSellersAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadSellersAsync();
                return Page();
            }

            var result = await Api.PutAsync($"/api/invoices/{Id}", Input);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                await LoadSellersAsync();
                return Page();
            }

            SuccessMessage = "Invoice updated and commission recalculated.";
            return RedirectToPage("Index");
        }

        private async Task LoadSellersAsync()
        {
            var sellers = await Api.GetAsync<List<SellerOption>>("/api/sellers/options");
            Sellers = sellers.Select(s => new SelectListItem(s.FullName, s.Id.ToString(), s.Id == Input.SellerId)).ToList();
        }
    }
}
