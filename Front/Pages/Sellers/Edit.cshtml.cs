using Front.Models;
using Front.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Front.Pages.Sellers
{
    [Authorize(Roles = FrontClaims.AdminRole)]
    public class EditModel : ApiPageModel
    {
        public EditModel(ApiClient api) : base(api) { }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public SellerEditForm Input { get; set; } = new();

        public string Cpf { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            var seller = await Api.FindAsync<SellerItem>($"/api/sellers/{Id}");

            if (seller == null)
            {
                return NotFound();
            }

            Cpf = seller.Cpf;
            Email = seller.Email;
            Input = new SellerEditForm { FullName = seller.FullName, CommissionRate = seller.CommissionRate };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await Api.PutAsync($"/api/sellers/{Id}", Input);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return Page();
            }

            SuccessMessage = "Seller updated.";
            return RedirectToPage("Index");
        }
    }
}
