using Microsoft.Extensions.DependencyInjection;

namespace Infra.DependencyInjection
{
    public static class InfraDependencyInjection
    {
        public static IServiceCollection AddInfra(
            this IServiceCollection services)
        {
            // Repositórios (exemplo)
             //services.AddScoped<IVendedorRepository, VendedorRepository>();
            // services.AddScoped<IInvoiceRepository, InvoiceRepository>();

            return services;
        }
    }
}
