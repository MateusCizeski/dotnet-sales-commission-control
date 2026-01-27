using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Configurations
{
    public class ComissaoConfiguration : IEntityTypeConfiguration<Comissao>
    {
        public void Configure(EntityTypeBuilder<Comissao> builder)
        {
            builder.ToTable("comissoes");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.ValorBase)
                .HasColumnType("numeric(18,2)")
                .IsRequired();

            builder.Property(c => c.PercentualAplicado)
                .HasColumnType("numeric(5,2)")
                .IsRequired();

            builder.Property(c => c.ValorComissao)
                .HasColumnType("numeric(18,2)")
                .IsRequired();

            builder.Property(c => c.Status)
                .IsRequired();

            builder.Property(c => c.DataCalculo)
                .IsRequired();

            builder.Property(c => c.DataPagamento);

            builder.HasIndex(c => c.InvoiceId)
                .IsUnique();
        }
    }
}
