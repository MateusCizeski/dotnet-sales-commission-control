using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Data.Configurations
{
    public class VendedorConfiguration : IEntityTypeConfiguration<Vendedor>
    {
        public void Configure(EntityTypeBuilder<Vendedor> builder)
        {
            builder.ToTable("vendedores");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.NomeCompleto)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(v => v.Cpf)
                .HasMaxLength(11)
                .IsRequired();

            builder.Property(v => v.Email)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(v => v.Telefone)
                .HasMaxLength(20);

            builder.Property(v => v.PercentualComissao)
                .IsRequired();

            builder.Property(v => v.Ativo)
                .IsRequired();

            builder.Property(v => v.DataCadastro)
                .IsRequired();

            builder.HasIndex(v => v.Cpf)
                .IsUnique();

            builder.HasIndex(v => v.Email)
                .IsUnique();
        }
    }
}
