using ApiBase.Application.ApplicationGuid;
using ApiBase.Repository.Extensions;
using Application.Abstractions;
using Application.Queries;
using Domain.Interfaces;
using Infra.Persistence;
using Infra.Persistence.Repositories;
using Infra.Queries;
using Infra.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfra(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PortalDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // ApiBase: IUnitOfWork (used by the commands too), IRepositoryBase<T> and the query options.
            services.AddApiBaseCore<PortalDbContext>(configureOptions: options => options.MaxPageSize = 100);

            // Write side
            services.AddScoped<ISellerRepository, SellerRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<ICommissionRepository, CommissionRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IInvoiceNumberGenerator, SqlServerInvoiceNumberGenerator>();
            services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();

            // Read side
            services.AddScoped<ISellerQueries, SellerQueries>();
            services.AddScoped<IApplicationGuid<InvoiceView>, InvoiceQueries>();
            services.AddScoped<IApplicationGuid<CommissionView>, CommissionQueries>();
            services.AddScoped<IDashboardQueries, DashboardQueries>();

            services.AddScoped<DevelopmentDataSeeder>();

            return services;
        }
    }
}
