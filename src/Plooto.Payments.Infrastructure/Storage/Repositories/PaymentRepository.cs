using Microsoft.EntityFrameworkCore;
using Plooto.Payments.Domain.Entities;
using Plooto.Payments.Domain.Interfaces;
using Plooto.Payments.Infrastructure.Storage.Configuration;

namespace Plooto.Payments.Infrastructure.Storage.Repositories
{
    public class PaymentRepository : Repository<Payment, int>, IPaymentRepository
    {
        public PaymentRepository(PaymentDbContext context) : base(context)
        {

        }

        public async Task<IEnumerable<Payment>> GetPaymentsByBillAsync(int billId, CancellationToken cancellationToken)
        {
            return await Context.Set<Payment>()
                                .Where(w => w.BillId == billId)
                                .OrderBy(o => o.DebitDate)
                                .ToListAsync(cancellationToken);
        }
    }
}
