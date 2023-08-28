using MediatR;
using Plooto.Payments.Domain.Interfaces.Events;

namespace Plooto.Payments.Infrastructure.Events
{
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;

        public DomainEventDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            //here, instead of calling a mediator, we could post into a topic/queue for example
            await _mediator.Publish(domainEvent, cancellationToken);
        }
    }
}
