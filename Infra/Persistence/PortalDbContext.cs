using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistence
{
    public class PortalDbContext : DbContext
    {
        /// <summary>SQL Server sequence that numbers the invoices (created by the InitialCreate migration).</summary>
        public const string InvoiceNumberSequence = "InvoiceNumberSeq";

        public PortalDbContext(DbContextOptions<PortalDbContext> options) : base(options) { }

        public DbSet<Seller> Sellers => Set<Seller>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Commission> Commissions => Set<Commission>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PortalDbContext).Assembly);
        }
    }
}
