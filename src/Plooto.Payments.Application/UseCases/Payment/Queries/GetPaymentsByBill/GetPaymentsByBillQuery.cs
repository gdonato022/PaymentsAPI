using MediatR;
using Microsoft.Extensions.Logging;
using Plooto.Payments.Domain.Interfaces;

namespace Plooto.Payments.Application.UseCases.Payment.Queries
{
    /// <summary>
    /// Get a list of payments by the given bill's id
    /// </summary>
    public class GetPaymentsByBillQuery : IRequest<IEnumerable<Domain.Entities.Payment>>
    {
        /// <summary>
        /// Bill's id
        /// </summary>
        public int BillId { get; set; }
    }

    /// <summary>
    /// GetBillsQuery handler
    /// </summary>
    public class GetPaymentsByBillQueryHandler : IRequestHandler<GetPaymentsByBillQuery, IEnumerable<Domain.Entities.Payment>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<GetPaymentsByBillQueryHandler> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="paymentRepository"></param>
        /// <param name="logger"></param>
        public GetPaymentsByBillQueryHandler(IPaymentRepository paymentRepository, ILogger<GetPaymentsByBillQueryHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handles GetPaymentsByBillQuery
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Domain.Entities.Payment>> Handle(GetPaymentsByBillQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("{@commandName} request received on handler {@request}", nameof(GetPaymentsByBillQuery), request);

            try
            {
                var payments = await _paymentRepository.GetPaymentsByBillAsync(request.BillId, cancellationToken);

                return payments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{@commandName} - Unable to get payments from the database.", nameof(GetPaymentsByBillQuery));

                throw;
            }
        }
    }
}
