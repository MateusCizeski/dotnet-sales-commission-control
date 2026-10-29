using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.TryAddSingleton(TimeProvider.System);

            services.AddScoped<ISellerService, SellerService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<ICommissionService, CommissionService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
