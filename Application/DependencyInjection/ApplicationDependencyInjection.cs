using Application.Applications;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DependencyInjection
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IVendedorApplication, VendedorApplication>();
            services.AddScoped<IInvoiceApplication, InvoiceApplication>();
            services.AddScoped<IComissaoApplication, ComissaoApplication>();
            services.AddScoped<IDashboardApplication, DashboardApplication>();

            return services;
        }
    }
}
