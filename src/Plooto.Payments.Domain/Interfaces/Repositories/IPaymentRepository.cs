using Plooto.Payments.Domain.Entities;

namespace Plooto.Payments.Domain.Interfaces
{
    public interface IPaymentRepository : IRepository<Payment, int>
    {
        Task<IEnumerable<Payment>> GetPaymentsByBillAsync(int billId, CancellationToken cancellationToken);
    }
}
