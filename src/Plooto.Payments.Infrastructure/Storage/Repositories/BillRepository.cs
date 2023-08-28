using Plooto.Payments.Domain.Entities;
using Plooto.Payments.Domain.Interfaces;
using Plooto.Payments.Infrastructure.Storage.Configuration;

namespace Plooto.Payments.Infrastructure.Storage.Repositories
{
    public class BillRepository : Repository<Bill, int>, IBillRepository
    {
        public BillRepository(PaymentDbContext context) : base(context)
        {
        }
    }
}
