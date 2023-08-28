using MediatR;
using Microsoft.Extensions.Logging;
using Plooto.Payments.Domain.Interfaces;

namespace Plooto.Payments.Application.UseCases.Bill.Commands
{
    /// <summary>
    /// Command to create a new bill
    /// </summary>
    public class CreateBillCommand : IRequest<int>
    {
        /// <summary>
        /// Vendor's name
        /// </summary>
        public string VendorName { get; set; }
        /// <summary>
        /// Bill's amount
        /// </summary>
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// CreateBillCommand handler
    /// </summary>
    public class CreateBillCommandHandler : IRequestHandler<CreateBillCommand, int>
    {
        private readonly IBillRepository _billRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateBillCommandHandler> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="billRepository"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="logger"></param>
        public CreateBillCommandHandler(IBillRepository billRepository, IUnitOfWork unitOfWork, ILogger<CreateBillCommandHandler> logger)
        {
            _billRepository = billRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Handles CreateBillCommand
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<int> Handle(CreateBillCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("{@commandName} request received on handler {@request}", nameof(CreateBillCommand), request);

            var bill = new Domain.Entities.Bill
            {
                VendorName = request.VendorName,
                Amount = request.Amount,
            };

            try
            {
                await _billRepository.AddAsync(bill, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return bill.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{@commandName} - Unable to insert bill on the database. Bill: {@bill}",
                    nameof(CreateBillCommand), bill);

                throw;
            }
        }
    }
}
