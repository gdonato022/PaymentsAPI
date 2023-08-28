namespace Plooto.Payments.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBillRepository BillRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
