using FluentValidation;
using Plooto.Payments.Domain.Interfaces;

namespace Plooto.Payments.Application.UseCases.Payment.Commands
{
    /// <summary>
    /// CreatePaymentCommand validator
    /// </summary>
    public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
    {
        private readonly IBillRepository _billRepository;
        /// <summary>
        /// A bill does not exists for the given id.
        /// </summary>
        public const string BILL_DOES_NOT_EXISTS = "A bill does not exists for the given id.";

        /// <summary>
        /// Constructor, performs validation over the command
        /// </summary>
        public CreatePaymentCommandValidator(IBillRepository billRepository)
        {
            _billRepository = billRepository;

            RuleFor(f => f.Amount)
                .GreaterThan(0);

            RuleFor(s => s.PaymentMethod)
               .IsInEnum();

            RuleFor(s => s.DebitDate)
               .Must(m => m!= DateTime.MinValue);

            RuleFor(s => s.BillId)
               .MustAsync(EnsureBillExists)
               .WithMessage(BILL_DOES_NOT_EXISTS);
        }

        private async Task<bool> EnsureBillExists(int billId, CancellationToken cancellationToken)
        {
            return await _billRepository.GetByIdAsync(billId, cancellationToken) != null;
        }
    }
}
