using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Front.Services
{
    public static class FrontClaims
    {
        /// <summary>The API access token, kept inside the encrypted, HttpOnly authentication cookie.</summary>
        public const string AccessToken = "access_token";
        public const string SellerId = "seller_id";
        public const string AdminRole = "Admin";
    }

    /// <summary>Adds "Authorization: Bearer {token}" to every call to the API.</summary>
    public class BearerTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BearerTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string? token = _httpContextAccessor.HttpContext?.User.FindFirstValue(FrontClaims.AccessToken);

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }

    /// <summary>When the API answers 401 (token expired), sign out of the Front and go to the login page.</summary>
    public class ApiUnauthorizedPageFilter : IAsyncPageFilter
    {
        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context) => Task.CompletedTask;

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            var executed = await next();

            if (executed.Exception is ApiUnauthorizedException)
            {
                executed.ExceptionHandled = true;
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                executed.Result = new RedirectToPageResult("/Account/Login", new { returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString });
            }
        }
    }
}
