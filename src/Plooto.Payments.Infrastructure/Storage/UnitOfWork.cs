using Plooto.Payments.Domain.Interfaces;
using Plooto.Payments.Infrastructure.Storage.Configuration;
using Plooto.Payments.Infrastructure.Storage.Repositories;

namespace Plooto.Payments.Infrastructure.Storage
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PaymentDbContext _context;
        private IBillRepository _billRepository;
        private IPaymentRepository _paymentRepository;

        public UnitOfWork(PaymentDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IBillRepository BillRepository => _billRepository ??= new BillRepository(_context);

        public IPaymentRepository PaymentRepository => _paymentRepository ??= new PaymentRepository(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
