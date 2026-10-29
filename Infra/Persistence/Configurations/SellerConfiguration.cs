using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Persistence.Configurations
{
    public class SellerConfiguration : IEntityTypeConfiguration<Seller>
    {
        public void Configure(EntityTypeBuilder<Seller> builder)
        {
            builder.ToTable("sellers");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.FullName)
                .HasMaxLength(Seller.FullNameMaxLength)
                .IsRequired();

            builder.Property(s => s.Cpf)
                .HasMaxLength(11)
                .IsRequired();

            builder.Property(s => s.Email)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(s => s.Phone)
                .HasMaxLength(20);

            // 0 to 15 with two decimals: decimal(5,2) is enough.
            builder.Property(s => s.CommissionRate)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(s => s.IsActive)
                .IsRequired();

            builder.Property(s => s.CreatedAt)
                .IsRequired();

            builder.HasIndex(s => s.Cpf)
                .IsUnique();

            builder.HasIndex(s => s.Email)
                .IsUnique();
        }
    }
}
