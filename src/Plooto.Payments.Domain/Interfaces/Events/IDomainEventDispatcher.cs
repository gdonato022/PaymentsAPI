namespace Plooto.Payments.Domain.Interfaces.Events
{
    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken);
    }
}
