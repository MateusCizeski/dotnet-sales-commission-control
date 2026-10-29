using Application.Abstractions;
using Infra.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Tests.Integration
{
    /// <summary>
    /// Runs the real API in memory (routing, auth, validation, exception handler, EF Core) over an
    /// in-memory SQLite database. Only two things are replaced: the SQL Server provider and the
    /// SQL Server SEQUENCE used for invoice numbers. All credentials here are fictitious test values.
    /// </summary>
    public class SalesApiFactory : WebApplicationFactory<Program>
    {
        public const string AdminEmail = "admin@example.com";
        public const string AdminPassword = "Admin-Test-Password-1";
        public const string SellerEmail = "seller@example.com";
        public const string SellerPassword = "Seller-Test-Password-1";

        private readonly SqliteConnection _connection = new("DataSource=:memory:");

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            _connection.Open();

            builder.UseEnvironment("Testing");
            builder.UseSetting("Jwt:Secret", "integration-tests-signing-key-with-at-least-32-bytes");
            builder.UseSetting("RateLimiting:LoginPermitLimit", "1000");
            builder.UseSetting("Seed:AdminEmail", AdminEmail);
            builder.UseSetting("Seed:AdminPassword", AdminPassword);
            builder.UseSetting("Seed:SellerEmail", SellerEmail);
            builder.UseSetting("Seed:SellerPassword", SellerPassword);

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<PortalDbContext>>();
                services.RemoveAll<IDbContextOptionsConfiguration<PortalDbContext>>();
                services.AddDbContext<PortalDbContext>(options => options
                    .UseSqlite(_connection)
                    .ReplaceService<IModelCustomizer, SqliteDecimalModelCustomizer>());

                services.RemoveAll<IInvoiceNumberGenerator>();
                services.AddSingleton<IInvoiceNumberGenerator, InMemoryInvoiceNumberGenerator>();
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);

            using var scope = host.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<PortalDbContext>().Database.EnsureCreated();
            scope.ServiceProvider.GetRequiredService<DevelopmentDataSeeder>()
                .SeedAsync(scope.ServiceProvider.GetRequiredService<IConfiguration>())
                .GetAwaiter().GetResult();

            return host;
        }

        public async Task<HttpClient> CreateClientAsAsync(string email, string password)
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
            response.EnsureSuccessStatusCode();

            var login = await response.Content.ReadFromJsonAsync<LoginResult>();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.AccessToken);
            return client;
        }

        public Task<HttpClient> CreateAdminClientAsync() => CreateClientAsAsync(AdminEmail, AdminPassword);

        public Task<HttpClient> CreateSellerClientAsync() => CreateClientAsAsync(SellerEmail, SellerPassword);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _connection.Dispose();
        }

        private record LoginResult(string AccessToken, Guid? SellerId);
    }

    /// <summary>SQLite has no decimal type: store decimals as REAL so SUM/ORDER BY work in the tests.</summary>
    public class SqliteDecimalModelCustomizer : RelationalModelCustomizer
    {
        public SqliteDecimalModelCustomizer(ModelCustomizerDependencies dependencies) : base(dependencies) { }

        public override void Customize(ModelBuilder modelBuilder, DbContext context)
        {
            base.Customize(modelBuilder, context);

            var decimals = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal));

            foreach (var property in decimals)
            {
                property.SetValueConverter(new ValueConverter<decimal, double>(v => (double)v, v => (decimal)v));
            }
        }
    }

    /// <summary>Replaces the SQL Server SEQUENCE in the tests.</summary>
    public class InMemoryInvoiceNumberGenerator : IInvoiceNumberGenerator
    {
        private long _current;

        public Task<string> NextAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult($"INV-{Interlocked.Increment(ref _current):D4}");
        }
    }
}
