using Front.Models;
using Front.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Front.Pages.Sellers
{
    [Authorize(Roles = FrontClaims.AdminRole)]
    public class CreateModel : ApiPageModel
    {
        public CreateModel(ApiClient api) : base(api) { }

        [BindProperty]
        public SellerForm Input { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await Api.PostAsync("/api/sellers", Input);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Error!);
                return Page();
            }

            SuccessMessage = "Seller created.";
            return RedirectToPage("Index");
        }
    }
}
