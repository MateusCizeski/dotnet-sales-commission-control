using Front.Models;
using Front.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Front.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly ApiClient _api;

        public LoginModel(ApiClient api)
        {
            _api = api;
        }

        [BindProperty]
        public LoginForm Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var login = await _api.LoginAsync(Input.Email, Input.Password);

            if (login == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, Input.Email),
                new(ClaimTypes.Role, login.Role),
                new(FrontClaims.AccessToken, login.AccessToken)
            };

            if (login.SellerId.HasValue)
            {
                claims.Add(new Claim(FrontClaims.SellerId, login.SellerId.Value.ToString()));
            }

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

            // The cookie expires with the API token: no session outlives its token.
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                ExpiresUtc = DateTime.SpecifyKind(login.ExpiresAt, DateTimeKind.Utc),
                IsPersistent = false
            });

            // Only local URLs: an external returnUrl would be an open redirect.
            return LocalRedirect(Url.IsLocalUrl(ReturnUrl) ? ReturnUrl! : "/Dashboard");
        }
    }
}
