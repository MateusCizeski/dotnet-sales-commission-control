using Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistence
{
    /// <summary>
    /// Numbers invoices with a SQL Server SEQUENCE: atomic and safe with several API instances,
    /// unlike "SELECT MAX(Number) + 1", which gives the same number to concurrent requests.
    /// A sequence can leave gaps (a number taken by a request that later failed) — acceptable here.
    /// </summary>
    public class SqlServerInvoiceNumberGenerator : IInvoiceNumberGenerator
    {
        private readonly PortalDbContext _context;

        public SqlServerInvoiceNumberGenerator(PortalDbContext context)
        {
            _context = context;
        }

        public async Task<string> NextAsync(CancellationToken cancellationToken = default)
        {
            // ToListAsync (not SingleAsync): any LINQ operator would wrap the SQL in a subquery,
            // and SQL Server does not allow NEXT VALUE FOR inside a derived table.
            var values = await _context.Database
                .SqlQueryRaw<long>("SELECT NEXT VALUE FOR " + PortalDbContext.InvoiceNumberSequence + " AS [Value]")
                .ToListAsync(cancellationToken);

            return $"INV-{values.Single():D4}";
        }
    }
}
