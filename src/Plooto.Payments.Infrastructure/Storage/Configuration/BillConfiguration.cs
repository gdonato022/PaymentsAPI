using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Plooto.Payments.Domain.Entities;

namespace Plooto.Payments.Infrastructure.Storage.Configuration
{
    public class BillConfiguration : IEntityTypeConfiguration<Bill>
    {
        public void Configure(EntityTypeBuilder<Bill> builder)
        {
            builder.HasKey(b => b.Id);
            builder.Property(b => b.VendorName).HasMaxLength(255).IsRequired();
            builder.Property(b => b.Amount).HasColumnType("decimal(18, 2)").IsRequired();
            builder.Property(b => b.IsPaid).IsRequired();
        }
    }
}
