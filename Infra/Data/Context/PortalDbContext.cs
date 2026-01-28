using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Context
{
    public class PortalDbContext : DbContext
    {
        public PortalDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Vendedor> Vendedores => Set<Vendedor>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Comissao> Comissoes => Set<Comissao>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PortalDbContext).Assembly);
        }
    }
}
