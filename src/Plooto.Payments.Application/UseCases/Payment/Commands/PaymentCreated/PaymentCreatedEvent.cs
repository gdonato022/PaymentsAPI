using MediatR;
using Microsoft.Extensions.Logging;
using Plooto.Payments.Domain.Interfaces;
using Plooto.Payments.Domain.Interfaces.Events;

namespace Plooto.Payments.Application.UseCases.Payment.Commands
{
    /// <summary>
    /// Event to check if bill is paid
    /// </summary>
    public class PaymentCreatedEvent : IDomainEvent, INotification
    {
        /// <summary>
        /// Payment
        /// </summary>
        public Domain.Entities.Payment Payment { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payment"></param>
        public PaymentCreatedEvent(Domain.Entities.Payment payment)
        {
            Payment = payment;
        }
    }

    /// <summary>
    /// PaymentCreatedEvent handler
    /// </summary>
    public class PaymentCreatedEventHandler : INotificationHandler<PaymentCreatedEvent>
    {
        private readonly IBillRepository _billRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PaymentCreatedEventHandler> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="billRepository"></param>
        /// <param name="paymentRepository"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="logger"></param>
        public PaymentCreatedEventHandler(IBillRepository billRepository,
                                          IPaymentRepository paymentRepository,
                                          IUnitOfWork unitOfWork,
                                          ILogger<PaymentCreatedEventHandler> logger)
        {
            _billRepository = billRepository;
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Handles PaymentCreatedEvent
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Handle(PaymentCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogDebug("{@commandName} request received on handler {@request}", nameof(PaymentCreatedEvent), notification);

            Domain.Entities.Bill bill = default;

            try
            {
                bill = await _billRepository.GetByIdAsync(notification.Payment.BillId, cancellationToken);

                if (bill != null)
                {
                    var payments = await _paymentRepository.GetPaymentsByBillAsync(bill.Id, cancellationToken);
                    var totalPayment = payments.Sum(s => s.Amount);

                    if (bill.Amount <= totalPayment)
                    {
                        _logger.LogDebug("{@commandName} setting bill {@bill} to paid...", nameof(PaymentCreatedEvent), bill.Id);

                        bill.IsPaid = true;

                        await _billRepository.UpdateAsync(bill);

                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{@commandName} - Unable to set bill as paid on the database. Bill: {@bill}",
                    nameof(PaymentCreatedEvent), bill);

                throw;
            }
        }
    }
}
