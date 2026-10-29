using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infra.Persistence
{
    /// <summary>
    /// Used only by the EF Core tools (dotnet ef migrations ...). Generating a migration does not
    /// connect to the database; "database update" uses the SALES_DB_CONNECTION environment variable.
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PortalDbContext>
    {
        public PortalDbContext CreateDbContext(string[] args)
        {
            string connectionString = Environment.GetEnvironmentVariable("SALES_DB_CONNECTION")
                ?? "Server=localhost;Database=SalesCommissionDb;Trusted_Connection=True;TrustServerCertificate=True";

            var options = new DbContextOptionsBuilder<PortalDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new PortalDbContext(options);
        }
    }
}
