using Domain.Interfaces;
using Domain.Services;
using Infra.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.DependencyInjection
{
    public static class InfraDependencyInjection
    {
        public static IServiceCollection AddInfra(
            this IServiceCollection services)
        {
            services.AddScoped<IVendedorRepository, VendedorRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IComissaoRepository, ComissaoRepository>();
            services.AddScoped<ComissaoService>();

            return services;
        }
    }
}
