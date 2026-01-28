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

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.Property(e => e.DataEmissao)
                      .HasColumnType("timestamp with time zone");
            });

            modelBuilder.Entity<Comissao>(entity =>
            {
                entity.Property(e => e.DataCalculo)
                      .HasColumnType("timestamp with time zone");

                entity.Property(e => e.DataPagamento)
                      .HasColumnType("timestamp with time zone");
            });

            modelBuilder.Entity<Vendedor>(entity =>
            {
                entity.Property(e => e.DataCadastro)
                      .HasColumnType("timestamp with time zone");
            });

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PortalDbContext).Assembly);
        }
    }
}
