using FluentValidation;

namespace Plooto.Payments.Application.UseCases.Payment.Commands
{
    /// <summary>
    /// PaymentCreatedEvent validator
    /// </summary>
    public class PaymentCreatedEventValidator : AbstractValidator<PaymentCreatedEvent>
    {
        /// <summary>
        /// Constructor, performs validation over the command
        /// </summary>
        public PaymentCreatedEventValidator()
        {
            RuleFor(f => f.Payment)
                .NotNull();
        }
    }
}
