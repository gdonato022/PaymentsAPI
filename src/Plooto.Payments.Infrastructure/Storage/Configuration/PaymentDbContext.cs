using Microsoft.EntityFrameworkCore;
using Plooto.Payments.Domain.Entities;

namespace Plooto.Payments.Infrastructure.Storage.Configuration
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {
        }

        public DbSet<Bill> Bills { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BillConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
        }
    }
}
