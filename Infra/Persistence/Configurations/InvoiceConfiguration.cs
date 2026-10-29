using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Persistence.Configurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.ToTable("invoices");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Number)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(i => i.IssueDate)
                .IsRequired();

            builder.Property(i => i.CustomerName)
                .HasMaxLength(Invoice.CustomerNameMaxLength)
                .IsRequired();

            builder.Property(i => i.CustomerTaxId)
                .HasMaxLength(14)
                .IsRequired();

            builder.Property(i => i.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(i => i.Status)
                .IsRequired();

            builder.Property(i => i.Notes)
                .HasMaxLength(Invoice.NotesMaxLength);

            builder.HasOne(i => i.Seller)
                .WithMany()
                .HasForeignKey(i => i.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Commission)
                .WithOne(c => c.Invoice)
                .HasForeignKey<Commission>(c => c.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(i => i.Number)
                .IsUnique();

            // Lists and the dashboard filter by seller, status and issue date.
            builder.HasIndex(i => new { i.SellerId, i.IssueDate });
            builder.HasIndex(i => i.Status);
        }
    }
}
