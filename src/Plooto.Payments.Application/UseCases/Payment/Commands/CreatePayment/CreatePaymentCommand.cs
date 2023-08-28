using MediatR;
using Microsoft.Extensions.Logging;
using Plooto.Payments.Domain.Enums;
using Plooto.Payments.Domain.Interfaces;
using Plooto.Payments.Domain.Interfaces.Events;

namespace Plooto.Payments.Application.UseCases.Payment.Commands
{
    /// <summary>
    /// Command to create a new payment
    /// </summary>
    public class CreatePaymentCommand : IRequest<int>
    {
        /// <summary>
        /// Bills' id
        /// </summary>
        public int BillId { get; set; }
        /// <summary>
        /// Payment's amount
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Payment's debit date
        /// </summary>
        public DateTime DebitDate { get; set; }
        /// <summary>
        /// Method of payment
        /// </summary>
        public PaymentMethod PaymentMethod { get; set; }
    }

    /// <summary>
    /// Handles CreatePaymentCommand
    /// </summary>
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, int>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreatePaymentCommandHandler> _logger;
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="paymentRepository"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="logger"></param>
        /// <param name="domainEventDispatcher"></param>
        public CreatePaymentCommandHandler(IPaymentRepository paymentRepository,
                                           IUnitOfWork unitOfWork,
                                           ILogger<CreatePaymentCommandHandler> logger,
                                           IDomainEventDispatcher domainEventDispatcher)
        {
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _domainEventDispatcher = domainEventDispatcher;
        }

        /// <summary>
        /// Handles CreatePaymentCommand
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("{@commandName} request received on handler {@request}", nameof(CreatePaymentCommand), request);

            var payment = new Domain.Entities.Payment
            {
                Amount = request.Amount,
                DebitDate = request.DebitDate,
                PaymentMethod = request.PaymentMethod,
                BillId = request.BillId,
            };

            try
            {
                await _paymentRepository.AddAsync(payment, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _domainEventDispatcher.DispatchAsync(new PaymentCreatedEvent(payment), cancellationToken);

                return payment.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{@commandName} - Unable to insert payment on the database. Payment: {@payment}",
                    nameof(CreatePaymentCommand), payment);

                throw;
            }
        }
    }
}
