using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.ToTable("invoices");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.NumeroInvoice)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(i => i.DataEmissao)
                .IsRequired();

            builder.Property(i => i.Cliente)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(i => i.CnpjCpfCliente)
                .HasMaxLength(14)
                .IsRequired();

            builder.Property(i => i.ValorTotal)
                .HasColumnType("numeric(18,2)")
                .IsRequired();

            builder.Property(i => i.Status)
                .IsRequired();

            builder.Property(i => i.Observacoes)
                .HasMaxLength(500);

            builder.HasOne(i => i.Vendedor)
                .WithMany()
                .HasForeignKey(i => i.VendedorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Comissao)
                .WithOne(c => c.Invoice)
                .HasForeignKey<Comissao>(c => c.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(i => i.NumeroInvoice)
                .IsUnique();
        }
    }
}
