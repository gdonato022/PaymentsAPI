using MediatR;
using Microsoft.Extensions.Logging;
using Plooto.Payments.Domain.Interfaces;

namespace Plooto.Payments.Application.UseCases.Bill.Queries
{
    /// <summary>
    /// Get a list of bills
    /// </summary>
    public class GetBillsQuery : IRequest<IEnumerable<Domain.Entities.Bill>>
    {

    }

    /// <summary>
    /// GetBillsQuery handler 
    /// </summary>
    public class GetBillsQueryHandler : IRequestHandler<GetBillsQuery, IEnumerable<Domain.Entities.Bill>>
    {
        private readonly IBillRepository _billRepository;
        private readonly ILogger<GetBillsQueryHandler> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="billRepository"></param>
        /// <param name="logger"></param>
        public GetBillsQueryHandler(IBillRepository billRepository, ILogger<GetBillsQueryHandler> logger)
        {
            _billRepository = billRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handles GetBillsQuery
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Domain.Entities.Bill>> Handle(GetBillsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("{@commandName} request received on handler {@request}", nameof(GetBillsQuery), request);

            try
            {
                var bills = await _billRepository.GetAllAsync(cancellationToken);

                return bills;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{@commandName} - Unable to get bills from the database.", nameof(GetBillsQuery));

                throw;
            }
        }
    }
}
