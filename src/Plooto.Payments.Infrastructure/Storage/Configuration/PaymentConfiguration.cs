using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Plooto.Payments.Domain.Entities;

namespace Plooto.Payments.Infrastructure.Storage.Configuration
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.BillId).IsRequired();
            builder.Property(p => p.Amount).HasColumnType("decimal(18, 2)").IsRequired();
            builder.Property(p => p.DebitDate).IsRequired();
            builder.Property(p => p.PaymentMethod).IsRequired();
        }
    }
}
