using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Persistence.Configurations
{
    public class CommissionConfiguration : IEntityTypeConfiguration<Commission>
    {
        public void Configure(EntityTypeBuilder<Commission> builder)
        {
            builder.ToTable("commissions");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.BaseAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(c => c.Rate)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(c => c.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(c => c.Status)
                .IsRequired();

            builder.Property(c => c.CalculatedAt)
                .IsRequired();

            builder.Property(c => c.PaidAt);

            builder.HasIndex(c => c.InvoiceId)
                .IsUnique();
        }
    }
}
